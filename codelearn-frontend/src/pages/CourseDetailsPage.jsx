import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import axiosClient from "../api/axiosClient";
import { useAuth } from "../context/AuthContext";

function CourseDetailsPage() {
    const { courseId } = useParams();
    const { isAuthenticated } = useAuth();

    const [course, setCourse] = useState(null);
    const [modules, setModules] = useState([]);
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        loadCourse();
        loadModules();
    }, [courseId]);

    const loadCourse = async () => {
        try {
            const response = await axiosClient.get(`/Courses/${courseId}`);
            setCourse(response.data);
        } catch {
            setError("Greška pri učitavanju kursa.");
        }
    };

    const loadModules = async () => {
        try {
            const response = await axiosClient.get(`/CourseModules/by-course/${courseId}`);
            setModules(response.data);
        } catch {
            setError("Greška pri učitavanju modula.");
        }
    };

    const enroll = async () => {
        setMessage("");
        setError("");

        if (!isAuthenticated) {
            setError("Moraš biti prijavljena da bi se upisala na kurs.");
            return;
        }

        try {
            await axiosClient.post(`/CourseEnrollments/enroll/${courseId}`);
            setMessage("Uspešno si upisana na kurs.");
        } catch {
            setError("Upis nije uspeo. Možda si već upisana na ovaj kurs.");
        }
    };

    if (!course) {
        return <p>Učitavanje...</p>;
    }

    return (
        <section>
            <div className="details-header">
                <h2>{course.title}</h2>
                <p>{course.description}</p>

                {message && <p className="success-message">{message}</p>}
                {error && <p className="error-message">{error}</p>}

                <button onClick={enroll}>Upiši kurs</button>
            </div>

            <h3>Moduli</h3>

            {modules.length === 0 && (
                <p className="empty-message">Ovaj kurs još nema module.</p>
            )}

            <div className="admin-list">
                {modules.map((module) => (
                    <div className="admin-list-item" key={module.id}>
                        <div>
                            <h4>{module.title}</h4>
                            <p>{module.description}</p>
                            <p>
                                <strong>Redni broj:</strong> {module.orderNumber}
                            </p>
                        </div>

                        <div className="admin-actions">
                            <Link
                                className="primary-link small-link"
                                to={`/modules/${module.id}/lessons`}
                            >
                                Lekcije
                            </Link>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default CourseDetailsPage;