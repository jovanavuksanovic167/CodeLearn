import { Link } from "react-router-dom";

function HomePage() {
    return (
        <section className="hero">
            <h1>Dobrodošli na CodeLearn</h1>

            <p>
                CodeLearn je platforma za učenje C# programskog jezika kroz lekcije,
                kvizove, dnevne izazove i praćenje napretka.
            </p>

            <div className="hero-actions">
                <Link className="primary-link" to="/courses">
                    Pogledaj kurseve
                </Link>

                <Link className="secondary-link" to="/register">
                    Registruj se
                </Link>
            </div>
        </section>
    );
}

export default HomePage;