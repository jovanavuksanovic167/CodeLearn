import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function Navbar() {
    const { user, isAuthenticated, isAdmin, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/");
    };

    return (
        <nav className="navbar">
            <div className="navbar-logo">
                <Link to="/">CodeLearn</Link>
            </div>

            <div className="navbar-links">
                <Link to="/">Početna</Link>
                <Link to="/courses">Kursevi</Link>

                {isAuthenticated && (
                    <>
                        <Link to="/my-progress">Moj napredak</Link>
                        <Link to="/daily-challenge">Daily Challenge</Link>
                    </>
                )}

                {isAdmin && (
                    <>
                        <Link to="/admin/courses">Kursevi</Link>
                        <Link to="/admin/modules">Moduli</Link>
                        <Link to="/admin/lessons">Lekcije</Link>
                        <Link to="/admin/quizzes">Kvizovi</Link>
                        <Link to="/admin/questions">Pitanja</Link>
                        <Link to="/admin/answers">Odgovori</Link>
                        <Link to="/admin/daily-challenges">Daily Admin</Link>
                    </>
                )}

                {!isAuthenticated && (
                    <>
                        <Link to="/login">Login</Link>
                        <Link to="/register">Register</Link>
                    </>
                )}

                {isAuthenticated && (
                    <>
                        <span className="navbar-user">
                            {user?.firstName} {user?.lastName}
                        </span>

                        <button className="logout-button" onClick={handleLogout}>
                            Logout
                        </button>
                    </>
                )}
            </div>
        </nav>
    );
}

export default Navbar;