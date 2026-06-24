import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminQuizzesPage() {
    const [lessons, setLessons] = useState([]);
    const [quizzes, setQuizzes] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        title: "",
        description: "",
        lessonId: "",
        timeLimit: 10,
        passingScore: 60
    });

    useEffect(() => {
        loadLessons();
        loadQuizzes();
    }, []);

    const loadLessons = async () => {
        try {
            const response = await axiosClient.get("/Lessons");
            setLessons(response.data);
        } catch {
            setError("Greška pri učitavanju lekcija.");
        }
    };

    const loadQuizzes = async () => {
        try {
            const response = await axiosClient.get("/Quizzes");
            setQuizzes(response.data);
        } catch {
            setError("Greška pri učitavanju kvizova.");
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;

        setForm({
            ...form,
            [name]: value
        });
    };

    const resetForm = () => {
        setEditingId(null);

        setForm({
            title: "",
            description: "",
            lessonId: "",
            timeLimit: 10,
            passingScore: 60
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            title: form.title,
            description: form.description,
            lessonId: Number(form.lessonId),
            timeLimit: Number(form.timeLimit),
            passingScore: Number(form.passingScore)
        };

        try {
            if (editingId) {
                await axiosClient.put(`/Quizzes/${editingId}`, dto);
                setMessage("Kviz je uspešno izmenjen.");
            } else {
                await axiosClient.post("/Quizzes", dto);
                setMessage("Kviz je uspešno dodat.");
            }

            resetForm();
            loadQuizzes();
        } catch {
            setError("Greška pri čuvanju kviza.");
        }
    };

    const handleEdit = (quiz) => {
        setEditingId(quiz.id);

        setForm({
            title: quiz.title,
            description: quiz.description,
            lessonId: quiz.lessonId,
            timeLimit: quiz.timeLimit,
            passingScore: quiz.passingScore
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovaj kviz?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/Quizzes/${id}`);
            setMessage("Kviz je obrisan.");
            loadQuizzes();
        } catch {
            setError("Greška pri brisanju kviza.");
        }
    };

    const getLessonTitle = (lessonId) => {
        const lesson = lessons.find((x) => x.id === lessonId);
        return lesson ? lesson.title : "Nepoznata lekcija";
    };

    return (
        <section>
            <h2>Admin - Upravljanje kvizovima</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni kviz" : "Dodaj kviz"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Lekcija</label>
                        <select
                            name="lessonId"
                            value={form.lessonId}
                            onChange={handleChange}
                        >
                            <option value="">Izaberi lekciju</option>

                            {lessons.map((lesson) => (
                                <option key={lesson.id} value={lesson.id}>
                                    {lesson.title}
                                </option>
                            ))}
                        </select>

                        <label>Naziv kviza</label>
                        <input
                            name="title"
                            type="text"
                            value={form.title}
                            onChange={handleChange}
                        />

                        <label>Opis kviza</label>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                        />

                        <label>Vremensko ograničenje u minutima</label>
                        <input
                            name="timeLimit"
                            type="number"
                            value={form.timeLimit}
                            onChange={handleChange}
                        />

                        <label>Passing score</label>
                        <input
                            name="passingScore"
                            type="number"
                            value={form.passingScore}
                            onChange={handleChange}
                        />

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj kviz"}
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
                    <h3>Lista kvizova</h3>

                    <div className="admin-list">
                        {quizzes.map((quiz) => (
                            <div className="admin-list-item" key={quiz.id}>
                                <div>
                                    <h4>{quiz.title}</h4>

                                    <p>{quiz.description}</p>

                                    <p>
                                        <strong>Lekcija:</strong>{" "}
                                        {getLessonTitle(quiz.lessonId)}
                                    </p>

                                    <p>
                                        <strong>Vreme:</strong> {quiz.timeLimit} min
                                    </p>

                                    <p>
                                        <strong>Passing score:</strong>{" "}
                                        {quiz.passingScore}%
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(quiz)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(quiz.id)}
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

export default AdminQuizzesPage;