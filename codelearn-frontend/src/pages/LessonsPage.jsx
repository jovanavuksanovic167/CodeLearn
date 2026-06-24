import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import axiosClient from "../api/axiosClient";

function LessonsPage() {
    const { moduleId } = useParams();

    const [lessons, setLessons] = useState([]);
    const [quizzesByLesson, setQuizzesByLesson] = useState({});
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        loadLessons();
    }, [moduleId]);

    const loadLessons = async () => {
        try {
            const response = await axiosClient.get(`/Lessons/by-module/${moduleId}`);
            const lessonsData = response.data;

            setLessons(lessonsData);

            const quizMap = {};

            for (const lesson of lessonsData) {
                const quizResponse = await axiosClient.get(`/Quizzes/by-lesson/${lesson.id}`);
                quizMap[lesson.id] = quizResponse.data;
            }

            setQuizzesByLesson(quizMap);
        } catch {
            setError("Greška pri učitavanju lekcija.");
        }
    };

    const completeLesson = async (lessonId) => {
        setMessage("");
        setError("");

        try {
            await axiosClient.post(`/LessonProgress/complete/${lessonId}`);
            setMessage("Lekcija je označena kao završena.");
        } catch {
            setError("Nije moguće završiti lekciju. Proveri da li si upisana na kurs.");
        }
    };

    return (
        <section>
            <h2>Lekcije</h2>

            {message && <p className="success-message">{message}</p>}
            {error && <p className="error-message">{error}</p>}

            {lessons.length === 0 && !error && (
                <p className="empty-message">Ovaj modul još nema lekcije.</p>
            )}

            <div className="admin-list">
                {lessons.map((lesson) => (
                    <div className="lesson-card" key={lesson.id}>
                        <h3>{lesson.title}</h3>

                        <p>{lesson.content}</p>

                        {lesson.codeExample && (
                            <pre className="code-block">
                                <code>{lesson.codeExample}</code>
                            </pre>
                        )}

                        <p>
                            <strong>Trajanje:</strong> {lesson.estimatedDuration} minuta
                        </p>

                        <div className="lesson-actions">
                            <button onClick={() => completeLesson(lesson.id)}>
                                Označi kao završenu
                            </button>

                            {quizzesByLesson[lesson.id]?.map((quiz) => (
                                <Link
                                    key={quiz.id}
                                    className="primary-link small-link"
                                    to={`/quiz/${quiz.id}`}
                                >
                                    Reši kviz: {quiz.title}
                                </Link>
                            ))}
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default LessonsPage;