import { Link } from "react-router-dom";

function CourseCard({ course }) {
    const getCourseLevelName = (level) => {
        if (level === 1) return "Beginner";
        if (level === 2) return "Intermediate";
        if (level === 3) return "Advanced";

        return "Unknown";
    };

    return (
        <div className="course-card">
            <h3>{course.title}</h3>

            <p>{course.description}</p>

            <p>
                <strong>Nivo:</strong> {getCourseLevelName(course.level)}
            </p>

            <p>
                <strong>Status:</strong> {course.isActive ? "Aktivan" : "Neaktivan"}
            </p>

            <Link className="primary-link small-link" to={`/courses/${course.id}`}>
                Detalji kursa
            </Link>
        </div>
    );
}

export default CourseCard;