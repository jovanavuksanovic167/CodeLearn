import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axiosClient from "../api/axiosClient";

function QuizPage() {
    const { quizId } = useParams();

    const [quiz, setQuiz] = useState(null);
    const [questions, setQuestions] = useState([]);
    const [optionsByQuestion, setOptionsByQuestion] = useState({});
    const [answers, setAnswers] = useState({});
    const [result, setResult] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {
        loadQuizData();
    }, [quizId]);

    const loadQuizData = async () => {
        try {
            const quizResponse = await axiosClient.get(`/Quizzes/${quizId}`);
            setQuiz(quizResponse.data);

            const questionsResponse = await axiosClient.get(`/Questions/by-quiz/${quizId}`);
            const questionData = questionsResponse.data;
            setQuestions(questionData);

            const optionsMap = {};

            for (const question of questionData) {
                const optionsResponse = await axiosClient.get(`/AnswerOptions/by-question/${question.id}`);
                optionsMap[question.id] = optionsResponse.data;
            }

            setOptionsByQuestion(optionsMap);
        } catch {
            setError("Greška pri učitavanju kviza.");
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

    const submitQuiz = async () => {
        setError("");
        setResult(null);

        const dto = {
            answers: Object.keys(answers).map((questionId) => ({
                questionId: Number(questionId),
                selectedAnswerOptionIds: answers[questionId]
            }))
        };

        try {
            const response = await axiosClient.post(`/Quizzes/${quizId}/submit`, dto);
            setResult(response.data);
        } catch {
            setError("Slanje kviza nije uspelo. Proveri da li si prijavljena.");
        }
    };

    if (!quiz) {
        return <p>Učitavanje...</p>;
    }

    return (
        <section>
            <h2>{quiz.title}</h2>
            <p>{quiz.description}</p>

            {error && <p className="error-message">{error}</p>}

            <div className="quiz-list">
                {questions.map((question) => (
                    <div className="question-card" key={question.id}>
                        <h3>{question.text}</h3>

                        <p>
                            <strong>Poeni:</strong> {question.points}
                        </p>

                        <div className="answer-options">
                            {optionsByQuestion[question.id]?.map((option) => {
                                const selectedAnswers = answers[question.id] || [];
                                const isMultipleChoice = question.questionType === 2;

                                return (
                                    <label className="answer-option" key={option.id}>
                                        <input
                                            type={isMultipleChoice ? "checkbox" : "radio"}
                                            name={`question-${question.id}`}
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

            <button onClick={submitQuiz}>Pošalji kviz</button>

            {result && (
                <div className="result-card">
                    <h3>Rezultat</h3>

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
        </section>
    );
}

export default QuizPage;