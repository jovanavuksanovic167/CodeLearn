import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminQuestionsPage() {
    const [quizzes, setQuizzes] = useState([]);
    const [questions, setQuestions] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        text: "",
        questionType: 1,
        points: 5,
        quizId: ""
    });

    useEffect(() => {
        loadQuizzes();
        loadQuestions();
    }, []);

    const loadQuizzes = async () => {
        try {
            const response = await axiosClient.get("/Quizzes");
            setQuizzes(response.data);
        } catch {
            setError("Greška pri učitavanju kvizova.");
        }
    };

    const loadQuestions = async () => {
        try {
            const response = await axiosClient.get("/Questions");
            setQuestions(response.data);
        } catch {
            setError("Greška pri učitavanju pitanja.");
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
            text: "",
            questionType: 1,
            points: 5,
            quizId: ""
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            text: form.text,
            questionType: Number(form.questionType),
            points: Number(form.points),
            quizId: Number(form.quizId)
        };

        try {
            if (editingId) {
                await axiosClient.put(`/Questions/${editingId}`, dto);
                setMessage("Pitanje je uspešno izmenjeno.");
            } else {
                await axiosClient.post("/Questions", dto);
                setMessage("Pitanje je uspešno dodato.");
            }

            resetForm();
            loadQuestions();
        } catch {
            setError("Greška pri čuvanju pitanja.");
        }
    };

    const handleEdit = (question) => {
        setEditingId(question.id);

        setForm({
            text: question.text,
            questionType: question.questionType,
            points: question.points,
            quizId: question.quizId
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovo pitanje?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/Questions/${id}`);
            setMessage("Pitanje je obrisano.");
            loadQuestions();
        } catch {
            setError("Greška pri brisanju pitanja.");
        }
    };

    const getQuizTitle = (quizId) => {
        const quiz = quizzes.find((x) => x.id === quizId);
        return quiz ? quiz.title : "Nepoznat kviz";
    };

    const getQuestionTypeName = (type) => {
        if (type === 1) return "SingleChoice";
        if (type === 2) return "MultipleChoice";
        if (type === 3) return "TrueFalse";

        return "Unknown";
    };

    return (
        <section>
            <h2>Admin - Upravljanje pitanjima</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni pitanje" : "Dodaj pitanje"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
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

                        <label>Tekst pitanja</label>
                        <textarea
                            name="text"
                            value={form.text}
                            onChange={handleChange}
                        />

                        <label>Tip pitanja</label>
                        <select
                            name="questionType"
                            value={form.questionType}
                            onChange={handleChange}
                        >
                            <option value={1}>SingleChoice</option>
                            <option value={2}>MultipleChoice</option>
                            <option value={3}>TrueFalse</option>
                        </select>

                        <label>Poeni</label>
                        <input
                            name="points"
                            type="number"
                            value={form.points}
                            onChange={handleChange}
                        />

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj pitanje"}
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
                    <h3>Lista pitanja</h3>

                    <div className="admin-list">
                        {questions.map((question) => (
                            <div className="admin-list-item" key={question.id}>
                                <div>
                                    <h4>{question.text}</h4>

                                    <p>
                                        <strong>Kviz:</strong>{" "}
                                        {getQuizTitle(question.quizId)}
                                    </p>

                                    <p>
                                        <strong>Tip:</strong>{" "}
                                        {getQuestionTypeName(question.questionType)}
                                    </p>

                                    <p>
                                        <strong>Poeni:</strong> {question.points}
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(question)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(question.id)}
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

export default AdminQuestionsPage;