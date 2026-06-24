import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";
import CourseCard from "../components/CourseCard";

function CoursesPage() {
    const [courses, setCourses] = useState([]);
    const [error, setError] = useState("");

    useEffect(() => {
        loadCourses();
    }, []);

    const loadCourses = async () => {
        try {
            const response = await axiosClient.get("/Courses");
            setCourses(response.data);
        } catch {
            setError("Greška pri učitavanju kurseva.");
        }
    };

    return (
        <section>
            <h2>Kursevi</h2>

            {error && <p className="error-message">{error}</p>}

            {courses.length === 0 && !error && (
                <p className="empty-message">Trenutno nema kurseva.</p>
            )}

            <div className="grid">
                {courses.map((course) => (
                    <CourseCard key={course.id} course={course} />
                ))}
            </div>
        </section>
    );
}

export default CoursesPage;