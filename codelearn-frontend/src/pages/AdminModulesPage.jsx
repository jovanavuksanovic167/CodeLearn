import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminModulesPage() {
    const [courses, setCourses] = useState([]);
    const [modules, setModules] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        title: "",
        description: "",
        orderNumber: 1,
        courseId: ""
    });

    useEffect(() => {
        loadCourses();
        loadModules();
    }, []);

    const loadCourses = async () => {
        try {
            const response = await axiosClient.get("/Courses");
            setCourses(response.data);
        } catch {
            setError("Greška pri učitavanju kurseva.");
        }
    };

    const loadModules = async () => {
        try {
            const response = await axiosClient.get("/CourseModules");
            setModules(response.data);
        } catch {
            setError("Greška pri učitavanju modula.");
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
            orderNumber: 1,
            courseId: ""
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            title: form.title,
            description: form.description,
            orderNumber: Number(form.orderNumber),
            courseId: Number(form.courseId)
        };

        try {
            if (editingId) {
                await axiosClient.put(`/CourseModules/${editingId}`, dto);
                setMessage("Modul je uspešno izmenjen.");
            } else {
                await axiosClient.post("/CourseModules", dto);
                setMessage("Modul je uspešno dodat.");
            }

            resetForm();
            loadModules();
        } catch {
            setError("Greška pri čuvanju modula.");
        }
    };

    const handleEdit = (module) => {
        setEditingId(module.id);

        setForm({
            title: module.title,
            description: module.description,
            orderNumber: module.orderNumber,
            courseId: module.courseId
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovaj modul?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/CourseModules/${id}`);
            setMessage("Modul je obrisan.");
            loadModules();
        } catch {
            setError("Greška pri brisanju modula.");
        }
    };

    const getCourseTitle = (courseId) => {
        const course = courses.find((x) => x.id === courseId);
        return course ? course.title : "Nepoznat kurs";
    };

    return (
        <section>
            <h2>Admin - Upravljanje modulima</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni modul" : "Dodaj modul"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Kurs</label>
                        <select
                            name="courseId"
                            value={form.courseId}
                            onChange={handleChange}
                        >
                            <option value="">Izaberi kurs</option>

                            {courses.map((course) => (
                                <option key={course.id} value={course.id}>
                                    {course.title}
                                </option>
                            ))}
                        </select>

                        <label>Naziv modula</label>
                        <input
                            name="title"
                            type="text"
                            value={form.title}
                            onChange={handleChange}
                        />

                        <label>Opis modula</label>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                        />

                        <label>Redni broj</label>
                        <input
                            name="orderNumber"
                            type="number"
                            value={form.orderNumber}
                            onChange={handleChange}
                        />

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj modul"}
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
                    <h3>Lista modula</h3>

                    <div className="admin-list">
                        {modules.map((module) => (
                            <div className="admin-list-item" key={module.id}>
                                <div>
                                    <h4>{module.title}</h4>

                                    <p>{module.description}</p>

                                    <p>
                                        <strong>Kurs:</strong>{" "}
                                        {getCourseTitle(module.courseId)}
                                    </p>

                                    <p>
                                        <strong>Redni broj:</strong>{" "}
                                        {module.orderNumber}
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(module)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(module.id)}
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

export default AdminModulesPage;