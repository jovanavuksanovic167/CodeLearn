import { createContext, useContext, useState } from "react";
import axiosClient from "../api/axiosClient";

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [token, setToken] = useState(() => localStorage.getItem("token"));

    const [user, setUser] = useState(() => {
        const savedUser = localStorage.getItem("user");
        return savedUser ? JSON.parse(savedUser) : null;
    });

    const login = async (email, password) => {
        const response = await axiosClient.post("/Auth/login", {
            email,
            password
        });

        const data = response.data;

        localStorage.setItem("token", data.token);
        localStorage.setItem("user", JSON.stringify(data));

        setToken(data.token);
        setUser(data);

        return data;
    };

    const register = async (firstName, lastName, email, password, confirmPassword) => {
        const response = await axiosClient.post("/Auth/register", {
            firstName,
            lastName,
            email,
            password,
            confirmPassword
        });

        const data = response.data;

        localStorage.setItem("token", data.token);
        localStorage.setItem("user", JSON.stringify(data));

        setToken(data.token);
        setUser(data);

        return data;
    };

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");

        setToken(null);
        setUser(null);
    };

    const isAuthenticated = !!token;
    const isAdmin = user?.roles?.includes("Admin");

    return (
        <AuthContext.Provider
            value={{
                token,
                user,
                login,
                register,
                logout,
                isAuthenticated,
                isAdmin
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}