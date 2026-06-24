import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminCoursesPage() {
    const [courses, setCourses] = useState([]);
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        title: "",
        description: "",
        level: 1,
        isActive: true
    });

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

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;

        setForm({
            ...form,
            [name]: type === "checkbox" ? checked : value
        });
    };

    const resetForm = () => {
        setEditingId(null);

        setForm({
            title: "",
            description: "",
            level: 1,
            isActive: true
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        try {
            if (editingId) {
                await axiosClient.put(`/Courses/${editingId}`, {
                    title: form.title,
                    description: form.description,
                    level: Number(form.level),
                    isActive: form.isActive
                });

                setMessage("Kurs je uspešno izmenjen.");
            } else {
                await axiosClient.post("/Courses", {
                    title: form.title,
                    description: form.description,
                    level: Number(form.level)
                });

                setMessage("Kurs je uspešno dodat.");
            }

            resetForm();
            loadCourses();
        } catch {
            setError("Greška pri čuvanju kursa. Proveri podatke ili admin prava.");
        }
    };

    const handleEdit = (course) => {
        setEditingId(course.id);

        setForm({
            title: course.title,
            description: course.description,
            level: course.level,
            isActive: course.isActive
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li si sigurna da želiš da obrišeš ovaj kurs?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/Courses/${id}`);
            setMessage("Kurs je obrisan.");
            loadCourses();
        } catch {
            setError("Greška pri brisanju kursa.");
        }
    };

    const getCourseLevelName = (level) => {
        if (level === 1) return "Beginner";
        if (level === 2) return "Intermediate";
        if (level === 3) return "Advanced";

        return "Unknown";
    };

    return (
        <section>
            <h2>Admin - Upravljanje kursevima</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni kurs" : "Dodaj kurs"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Naziv kursa</label>
                        <input
                            name="title"
                            type="text"
                            value={form.title}
                            onChange={handleChange}
                        />

                        <label>Opis kursa</label>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                        />

                        <label>Nivo</label>
                        <select
                            name="level"
                            value={form.level}
                            onChange={handleChange}
                        >
                            <option value={1}>Beginner</option>
                            <option value={2}>Intermediate</option>
                            <option value={3}>Advanced</option>
                        </select>

                        {editingId && (
                            <label className="checkbox-label">
                                <input
                                    name="isActive"
                                    type="checkbox"
                                    checked={form.isActive}
                                    onChange={handleChange}
                                />
                                Aktivan kurs
                            </label>
                        )}

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj kurs"}
                        </button>

                        {editingId && (
                            <button
                                type="button"
                                className="secondary-button"
                                onClick={resetForm}
                            >
                                Otkaži izmenu
                            </button>
                        )}
                    </form>
                </div>

                <div>
                    <h3>Lista kurseva</h3>

                    <div className="admin-list">
                        {courses.map((course) => (
                            <div className="admin-list-item" key={course.id}>
                                <div>
                                    <h4>{course.title}</h4>
                                    <p>{course.description}</p>

                                    <p>
                                        <strong>Nivo:</strong>{" "}
                                        {getCourseLevelName(course.level)}
                                    </p>

                                    <p>
                                        <strong>Status:</strong>{" "}
                                        {course.isActive ? "Aktivan" : "Neaktivan"}
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(course)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(course.id)}
                                    >
                                        Obriši
                                    </button>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </section>
    );
}

export default AdminCoursesPage;