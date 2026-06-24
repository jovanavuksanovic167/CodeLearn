import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

function AdminAnswerOptionsPage() {
    const [questions, setQuestions] = useState([]);
    const [answerOptions, setAnswerOptions] = useState([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const [editingId, setEditingId] = useState(null);

    const [form, setForm] = useState({
        text: "",
        isCorrect: false,
        questionId: ""
    });

    useEffect(() => {
        loadQuestions();
        loadAnswerOptions();
    }, []);

    const loadQuestions = async () => {
        try {
            const response = await axiosClient.get("/Questions");
            setQuestions(response.data);
        } catch {
            setError("Greška pri učitavanju pitanja.");
        }
    };

    const loadAnswerOptions = async () => {
        try {
            const response = await axiosClient.get("/AnswerOptions");
            setAnswerOptions(response.data);
        } catch {
            setError("Greška pri učitavanju ponuđenih odgovora.");
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
            text: "",
            isCorrect: false,
            questionId: ""
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setMessage("");
        setError("");

        const dto = {
            text: form.text,
            isCorrect: form.isCorrect,
            questionId: Number(form.questionId)
        };

        try {
            if (editingId) {
                await axiosClient.put(`/AnswerOptions/${editingId}`, dto);
                setMessage("Odgovor je uspešno izmenjen.");
            } else {
                await axiosClient.post("/AnswerOptions", dto);
                setMessage("Odgovor je uspešno dodat.");
            }

            resetForm();
            loadAnswerOptions();
        } catch {
            setError("Greška pri čuvanju odgovora. Proveri pravila za SingleChoice/TrueFalse.");
        }
    };

    const handleEdit = (answer) => {
        setEditingId(answer.id);

        setForm({
            text: answer.text,
            isCorrect: answer.isCorrect,
            questionId: answer.questionId
        });

        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id) => {
        const confirmed = window.confirm("Da li želiš da obrišeš ovaj odgovor?");

        if (!confirmed) {
            return;
        }

        setMessage("");
        setError("");

        try {
            await axiosClient.delete(`/AnswerOptions/${id}`);
            setMessage("Odgovor je obrisan.");
            loadAnswerOptions();
        } catch {
            setError("Greška pri brisanju odgovora.");
        }
    };

    const getQuestionText = (questionId) => {
        const question = questions.find((x) => x.id === questionId);
        return question ? question.text : "Nepoznato pitanje";
    };

    return (
        <section>
            <h2>Admin - Upravljanje ponuđenim odgovorima</h2>

            <div className="admin-layout">
                <div className="form-card">
                    <h3>{editingId ? "Izmeni odgovor" : "Dodaj odgovor"}</h3>

                    {message && <p className="success-message">{message}</p>}
                    {error && <p className="error-message">{error}</p>}

                    <form onSubmit={handleSubmit}>
                        <label>Pitanje</label>
                        <select
                            name="questionId"
                            value={form.questionId}
                            onChange={handleChange}
                        >
                            <option value="">Izaberi pitanje</option>

                            {questions.map((question) => (
                                <option key={question.id} value={question.id}>
                                    {question.text}
                                </option>
                            ))}
                        </select>

                        <label>Tekst odgovora</label>
                        <input
                            name="text"
                            type="text"
                            value={form.text}
                            onChange={handleChange}
                        />

                        <label className="checkbox-label">
                            <input
                                name="isCorrect"
                                type="checkbox"
                                checked={form.isCorrect}
                                onChange={handleChange}
                            />
                            Tačan odgovor
                        </label>

                        <button type="submit">
                            {editingId ? "Sačuvaj izmene" : "Dodaj odgovor"}
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
                    <h3>Lista ponuđenih odgovora</h3>

                    <div className="admin-list">
                        {answerOptions.map((answer) => (
                            <div className="admin-list-item" key={answer.id}>
                                <div>
                                    <h4>{answer.text}</h4>

                                    <p>
                                        <strong>Pitanje:</strong>{" "}
                                        {getQuestionText(answer.questionId)}
                                    </p>

                                    <p>
                                        <strong>Tačan:</strong>{" "}
                                        {answer.isCorrect ? "Da" : "Ne"}
                                    </p>
                                </div>

                                <div className="admin-actions">
                                    <button onClick={() => handleEdit(answer)}>
                                        Izmeni
                                    </button>

                                    <button
                                        className="danger-button"
                                        onClick={() => handleDelete(answer.id)}
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

export default AdminAnswerOptionsPage;