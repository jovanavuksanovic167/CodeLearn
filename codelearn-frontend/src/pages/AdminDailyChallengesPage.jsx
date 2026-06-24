import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminDailyChallengesPage() {
    const [quizzes, setQuizzes] = useState([]);
    const [challenges, setChallenges] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        date: "",
        quizId: "",
        isActive: false
    });

    useEffect(() => {
        loadQuizzes();
        loadChallenges();
    }, []);

    const loadQuizzes = async () => {
        try {
            const response = await axiosClient.get("/Quizzes");
            setQuizzes(response.data);
        } catch {
            setError("Greška pri učitavanju kvizova.");
        }
    };

    const loadChallenges = async () => {
        try {
            const response = await axiosClient.get("/DailyChallenges");
            setChallenges(response.data);
        } catch {
            setError("Greška pri učitavanju dnevnih izazova.");
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
            date: "",
            quizId: "",
            isActive: false
        });
    };

    const formatDateForInput = (dateValue) => {
        if (!dateValue) {
            return "";
        }

        return dateValue.substring(0, 10);
    };

    const buildDateForApi = (dateValue) => {
        return `${dateValue}T00:00:00Z`;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            date: buildDateForApi(form.date),
            quizId: Number(form.quizId),
            isActive: editingId ? form.isActive : false
        };

        try {
            if (editingId) {
                await axiosClient.put(`/DailyChallenges/${editingId}`, dto);
                setMessage("Daily Challenge je uspešno izmenjen.");
            } else {
                await axiosClient.post("/DailyChallenges", dto);
                setMessage("Daily Challenge je uspešno dodat. Hangfire će ga automatski aktivirati kada dođe taj datum.");
            }

            resetForm();
            loadChallenges();
        } catch {
            setError("Greška pri čuvanju Daily Challenge-a. Proveri da li već postoji challenge za taj datum.");
        }
    };

    const handleEdit = (challenge) => {
        setEditingId(challenge.id);

        setForm({
            date: formatDateForInput(challenge.date),
            quizId: challenge.quizId,
            isActive: challenge.isActive
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovaj Daily Challenge?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/DailyChallenges/${id}`);
            setMessage("Daily Challenge je obrisan.");
            loadChallenges();
        } catch {
            setError("Greška pri brisanju Daily Challenge-a.");
        }
    };

    const getQuizTitle = (quizId) => {
        const quiz = quizzes.find((x) => x.id === quizId);
        return quiz ? quiz.title : "Nepoznat kviz";
    };

    return (
        <section>
            <h2>Admin - Daily Challenge</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni Daily Challenge" : "Dodaj Daily Challenge"}</h3>

                    

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Datum</label>
                        <input
                            name="date"
                            type="date"
                            value={form.date}
                            onChange={handleChange}
                        />

                        <label>Kviz</label>
                        <select
                            name="quizId"
                            value={form.quizId}
                            onChange={handleChange}
                        >
                            <option value="">Izaberi kviz</option>

                            {quizzes.map((quiz) => (
                                <option key={quiz.id} value={quiz.id}>
                                    {quiz.title}
                                </option>
                            ))}
                        </select>

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj Daily Challenge"}
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
                    <h3>Lista Daily Challenge-a</h3>

                    {challenges.length === 0 && (
                        <p className="empty-message">
                            Još nema kreiranih Daily Challenge-a.
                        </p>
                    )}

                    <div className="admin-list">
                        {challenges.map((challenge) => (
                            <div className="admin-list-item" key={challenge.id}>
                                <div>
                                    <h4>{challenge.quizTitle || getQuizTitle(challenge.quizId)}</h4>

                                    <p>
                                        <strong>Datum:</strong>{" "}
                                        {new Date(challenge.date).toLocaleDateString()}
                                    </p>

                                    <p>
                                        <strong>Kviz:</strong>{" "}
                                        {getQuizTitle(challenge.quizId)}
                                    </p>

                                    <p>
                                        <strong>Aktivan:</strong>{" "}
                                        {challenge.isActive ? "Da" : "Ne"}
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(challenge)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(challenge.id)}
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

export default AdminDailyChallengesPage;