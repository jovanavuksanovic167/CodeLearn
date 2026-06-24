import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function MyProgressPage() {
    const [enrollments, setEnrollments] = useState([]);
    const [lessonProgress, setLessonProgress] = useState([]);
    const [quizResults, setQuizResults] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        loadProgress();
    }, []);

    const loadProgress = async () => {
        try {
            const enrollmentsResponse = await axiosClient.get("/CourseEnrollments/my-courses");
            setEnrollments(enrollmentsResponse.data);

            const lessonProgressResponse = await axiosClient.get("/LessonProgress/my-progress");
            setLessonProgress(lessonProgressResponse.data);

            const quizResultsResponse = await axiosClient.get("/QuizResults/my-results");
            setQuizResults(quizResultsResponse.data);
        } catch {
            setError("Greška pri učitavanju napretka.");
        }
    };

    return (
        <section>
            <h2>Moj napredak</h2>

            {error && <p className="error-message">{error}</p>}

            <h3>Moji kursevi</h3>

            {enrollments.length === 0 && (
                <p className="empty-message">Nisi upisana ni na jedan kurs.</p>
            )}

            <div className="admin-list">
                {enrollments.map((item) => (
                    <div className="admin-list-item" key={item.id}>
                        <div>
                            <h4>{item.courseTitle}</h4>

                            <p>
                                <strong>Status:</strong> {item.status}
                            </p>
                        </div>
                    </div>
                ))}
            </div>

            <h3>Moje završene lekcije</h3>

            {lessonProgress.length === 0 && (
                <p className="empty-message">Još nema završenih lekcija.</p>
            )}

            <div className="admin-list">
                {lessonProgress.map((item) => (
                    <div className="admin-list-item" key={item.id}>
                        <div>
                            <h4>{item.lessonTitle}</h4>

                            <p>
                                <strong>Završeno:</strong> {item.isCompleted ? "Da" : "Ne"}
                            </p>
                        </div>
                    </div>
                ))}
            </div>

            <h3>Rezultati kvizova</h3>

            {quizResults.length === 0 && (
                <p className="empty-message">Još nema rešenih kvizova.</p>
            )}

            <div className="admin-list">
                {quizResults.map((item) => (
                    <div className="admin-list-item" key={item.id}>
                        <div>
                            <h4>{item.quizTitle}</h4>

                            <p>
                                <strong>Score:</strong> {item.score}%
                            </p>

                            <p>
                                <strong>Tačni odgovori:</strong> {item.correctAnswers} / {item.totalQuestions}
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

export default MyProgressPage;