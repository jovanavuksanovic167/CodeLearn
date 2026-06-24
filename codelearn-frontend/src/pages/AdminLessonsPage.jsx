import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminLessonsPage() {
    const [modules, setModules] = useState([]);
    const [lessons, setLessons] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        title: "",
        content: "",
        codeExample: "",
        orderNumber: 1,
        estimatedDuration: 10,
        courseModuleId: ""
    });

    useEffect(() => {
        loadModules();
        loadLessons();
    }, []);

    const loadModules = async () => {
        try {
            const response = await axiosClient.get("/CourseModules");
            setModules(response.data);
        } catch {
            setError("Greška pri učitavanju modula.");
        }
    };

    const loadLessons = async () => {
        try {
            const response = await axiosClient.get("/Lessons");
            setLessons(response.data);
        } catch {
            setError("Greška pri učitavanju lekcija.");
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
            content: "",
            codeExample: "",
            orderNumber: 1,
            estimatedDuration: 10,
            courseModuleId: ""
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            title: form.title,
            content: form.content,
            codeExample: form.codeExample,
            orderNumber: Number(form.orderNumber),
            estimatedDuration: Number(form.estimatedDuration),
            courseModuleId: Number(form.courseModuleId)
        };

        try {
            if (editingId) {
                await axiosClient.put(`/Lessons/${editingId}`, dto);
                setMessage("Lekcija je uspešno izmenjena.");
            } else {
                await axiosClient.post("/Lessons", dto);
                setMessage("Lekcija je uspešno dodata.");
            }

            resetForm();
            loadLessons();
        } catch {
            setError("Greška pri čuvanju lekcije.");
        }
    };

    const handleEdit = (lesson) => {
        setEditingId(lesson.id);

        setForm({
            title: lesson.title,
            content: lesson.content,
            codeExample: lesson.codeExample || "",
            orderNumber: lesson.orderNumber,
            estimatedDuration: lesson.estimatedDuration,
            courseModuleId: lesson.courseModuleId
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovu lekciju?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/Lessons/${id}`);
            setMessage("Lekcija je obrisana.");
            loadLessons();
        } catch {
            setError("Greška pri brisanju lekcije.");
        }
    };

    const getModuleTitle = (moduleId) => {
        const module = modules.find((x) => x.id === moduleId);
        return module ? module.title : "Nepoznat modul";
    };

    return (
        <section>
            <h2>Admin - Upravljanje lekcijama</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni lekciju" : "Dodaj lekciju"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Modul</label>
                        <select
                            name="courseModuleId"
                            value={form.courseModuleId}
                            onChange={handleChange}
                        >
                            <option value="">Izaberi modul</option>

                            {modules.map((module) => (
                                <option key={module.id} value={module.id}>
                                    {module.title}
                                </option>
                            ))}
                        </select>

                        <label>Naziv lekcije</label>
                        <input
                            name="title"
                            type="text"
                            value={form.title}
                            onChange={handleChange}
                        />

                        <label>Sadržaj lekcije</label>
                        <textarea
                            name="content"
                            value={form.content}
                            onChange={handleChange}
                        />

                        <label>Primer koda</label>
                        <textarea
                            name="codeExample"
                            value={form.codeExample}
                            onChange={handleChange}
                        />

                        <label>Redni broj</label>
                        <input
                            name="orderNumber"
                            type="number"
                            value={form.orderNumber}
                            onChange={handleChange}
                        />

                        <label>Procena trajanja u minutima</label>
                        <input
                            name="estimatedDuration"
                            type="number"
                            value={form.estimatedDuration}
                            onChange={handleChange}
                        />

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj lekciju"}
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
                    <h3>Lista lekcija</h3>

                    <div className="admin-list">
                        {lessons.map((lesson) => (
                            <div className="admin-list-item" key={lesson.id}>
                                <div>
                                    <h4>{lesson.title}</h4>

                                    <p>{lesson.content}</p>

                                    <p>
                                        <strong>Modul:</strong>{" "}
                                        {getModuleTitle(lesson.courseModuleId)}
                                    </p>

                                    <p>
                                        <strong>Redni broj:</strong>{" "}
                                        {lesson.orderNumber}
                                    </p>

                                    <p>
                                        <strong>Trajanje:</strong>{" "}
                                        {lesson.estimatedDuration} minuta
                                    </p>

                                    {lesson.codeExample && (
                                        <pre className="code-block">
                                            <code>{lesson.codeExample}</code>
                                        </pre>
                                    )}
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(lesson)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(lesson.id)}
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

export default AdminLessonsPage;