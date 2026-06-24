import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function DailyChallengePage() {
    const [challenge, setChallenge] = useState(null);
    const [questions, setQuestions] = useState([]);
    const [optionsByQuestion, setOptionsByQuestion] = useState({});
    const [answers, setAnswers] = useState({});
    const [submissions, setSubmissions] = useState([]);
    const [result, setResult] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {
        loadDailyChallenge();
        loadSubmissions();
    }, []);

    const loadDailyChallenge = async () => {
        try {
            const response = await axiosClient.get("/DailyChallenges/today");
            setChallenge(response.data);

            await loadQuizQuestions(response.data.quizId);
        } catch {
            setError("Trenutno nema aktivnog dnevnog izazova.");
        }
    };

    const loadQuizQuestions = async (quizId) => {
        const questionsResponse = await axiosClient.get(`/Questions/by-quiz/${quizId}`);
        const questionData = questionsResponse.data;

        setQuestions(questionData);

        const optionsMap = {};

        for (const question of questionData) {
            const optionsResponse = await axiosClient.get(`/AnswerOptions/by-question/${question.id}`);
            optionsMap[question.id] = optionsResponse.data;
        }

        setOptionsByQuestion(optionsMap);
    };

    const loadSubmissions = async () => {
        try {
            const response = await axiosClient.get("/DailyChallenges/my-submissions");
            setSubmissions(response.data);
        } catch {
            setSubmissions([]);
        }
    };

    const handleSingleAnswer = (questionId, answerOptionId) => {
        setAnswers({
            ...answers,
            [questionId]: [answerOptionId]
        });
    };

    const handleMultipleAnswer = (questionId, answerOptionId) => {
        const currentAnswers = answers[questionId] || [];

        let updatedAnswers;

        if (currentAnswers.includes(answerOptionId)) {
            updatedAnswers = currentAnswers.filter((id) => id !== answerOptionId);
        } else {
            updatedAnswers = [...currentAnswers, answerOptionId];
        }

        setAnswers({
            ...answers,
            [questionId]: updatedAnswers
        });
    };

    const submitDailyChallenge = async () => {
        setError("");
        setResult(null);

        const dto = {
            answers: Object.keys(answers).map((questionId) => ({
                questionId: Number(questionId),
                selectedAnswerOptionIds: answers[questionId]
            }))
        };

        try {
            const response = await axiosClient.post(`/DailyChallenges/${challenge.id}/submit`, dto);
            setResult(response.data);
            loadSubmissions();
        } catch {
            setError("Slanje dnevnog izazova nije uspelo. Možda si ga već rešila.");
        }
    };

    return (
        <section>
            <h2>Daily Challenge</h2>

            {error && <p className="error-message">{error}</p>}

            {challenge && (
                <>
                    <div className="details-header">
                        <h3>{challenge.quizTitle}</h3>

                        <p>
                            <strong>Datum:</strong>{" "}
                            {new Date(challenge.date).toLocaleDateString()}
                        </p>
                    </div>

                    <div className="quiz-list">
                        {questions.map((question) => (
                            <div className="question-card" key={question.id}>
                                <h3>{question.text}</h3>

                                <div className="answer-options">
                                    {optionsByQuestion[question.id]?.map((option) => {
                                        const selectedAnswers = answers[question.id] || [];
                                        const isMultipleChoice = question.questionType === 2;

                                        return (
                                            <label className="answer-option" key={option.id}>
                                                <input
                                                    type={isMultipleChoice ? "checkbox" : "radio"}
                                                    name={`daily-question-${question.id}`}
                                                    checked={selectedAnswers.includes(option.id)}
                                                    onChange={() =>
                                                        isMultipleChoice
                                                            ? handleMultipleAnswer(question.id, option.id)
                                                            : handleSingleAnswer(question.id, option.id)
                                                    }
                                                />

                                                {option.text}
                                            </label>
                                        );
                                    })}
                                </div>
                            </div>
                        ))}
                    </div>

                    <button onClick={submitDailyChallenge}>
                        Pošalji Daily Challenge
                    </button>
                </>
            )}

            {result && (
                <div className="result-card">
                    <h3>Rezultat dnevnog izazova</h3>

                    <p>
                        <strong>Score:</strong> {result.score}%
                    </p>

                    <p>
                        <strong>Tačni odgovori:</strong> {result.correctAnswers} / {result.totalQuestions}
                    </p>

                    <p>
                        <strong>Status:</strong> {result.isPassed ? "Položeno" : "Nije položeno"}
                    </p>
                </div>
            )}

            <h3>Moji Daily Challenge rezultati</h3>

            {submissions.length === 0 && (
                <p className="empty-message">Još nema rezultata.</p>
            )}

            <div className="admin-list">
                {submissions.map((item) => (
                    <div className="admin-list-item" key={item.id}>
                        <div>
                            <h4>{item.quizTitle}</h4>

                            <p>
                                <strong>Datum:</strong>{" "}
                                {new Date(item.dailyChallengeDate).toLocaleDateString()}
                            </p>

                            <p>
                                <strong>Score:</strong> {item.score}%
                            </p>

                            <p>
                                <strong>Status:</strong> {item.isPassed ? "Položeno" : "Nije položeno"}
                            </p>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default DailyChallengePage;