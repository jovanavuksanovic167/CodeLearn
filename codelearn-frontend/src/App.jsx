import { Route, Routes } from "react-router-dom";
import Navbar from "./components/Navbar";
import ProtectedRoute from "./components/ProtectedRoute";

import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import CoursesPage from "./pages/CoursesPage";
import CourseDetailsPage from "./pages/CourseDetailsPage";
import LessonsPage from "./pages/LessonsPage";
import QuizPage from "./pages/QuizPage";
import MyProgressPage from "./pages/MyProgressPage";
import DailyChallengePage from "./pages/DailyChallengePage";

import AdminCoursesPage from "./pages/AdminCoursesPage";
import AdminModulesPage from "./pages/AdminModulesPage";
import AdminLessonsPage from "./pages/AdminLessonsPage";
import AdminQuizzesPage from "./pages/AdminQuizzesPage";
import AdminQuestionsPage from "./pages/AdminQuestionsPage";
import AdminAnswerOptionsPage from "./pages/AdminAnswerOptionsPage";
import AdminDailyChallengesPage from "./pages/AdminDailyChallengesPage";

function App() {
    return (
        <>
            <Navbar />

            <main className="container">
                <Routes>
                    <Route path="/" element={<HomePage />} />

                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    <Route path="/courses" element={<CoursesPage />} />
                    <Route path="/courses/:courseId" element={<CourseDetailsPage />} />
                    <Route path="/modules/:moduleId/lessons" element={<LessonsPage />} />
                    <Route path="/quiz/:quizId" element={<QuizPage />} />

                    <Route
                        path="/my-progress"
                        element={
                            <ProtectedRoute>
                                <MyProgressPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/daily-challenge"
                        element={
                            <ProtectedRoute>
                                <DailyChallengePage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/courses"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminCoursesPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/modules"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminModulesPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/lessons"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminLessonsPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/quizzes"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminQuizzesPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/questions"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminQuestionsPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/answers"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminAnswerOptionsPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route
                        path="/admin/daily-challenges"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminDailyChallengesPage />
                            </ProtectedRoute>
                        }
                    />
                </Routes>
            </main>
        </>
    );
}

export default App;