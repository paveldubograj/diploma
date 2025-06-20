import React, { useEffect } from "react";
import { Container } from "react-bootstrap";
import LoginForm from "./components/LoginComponents/LoginForm";
import { Route, BrowserRouter as Router, Routes, useNavigate } from "react-router-dom";
import NewsPage from "./Pages/NewsPages/NewsPage";
import NewsDetails from "./Pages/NewsPages/NewsDetailPage";
import NewsCreatePage from "./Pages/NewsPages/NewsCreatePage";
import RegisterPage from "./Pages/UserPages/RegisterPage";
import ProfilePage from "./Pages/UserPages/ProfilePage";
import UserList from "./Pages/UserPages/UserList";
import UserDetailsAdmin from "./Pages/UserPages/UserDetailsAdmin";
import MatchesPage from "./Pages/MatchPages/MatchesPage";
import MatchDetails from "./components/MatchComponents/MatchDetails";
import TournamentList from "./components/TournamentComponents/TournamentList";
import TournamentDetails from "./components/TournamentComponents/TournamentDetails";
import AddParticipantPage from "./components/TournamentComponents/AddParticipantPage";
import ParticipantEdit from "./components/TournamentComponents/ParticipantEdit";
import { AuthProvider } from "./api/AuthHook";
import Header from "./components/Header";
import Footer from "./components/Footer";
import CreateTournament from "./components/TournamentComponents/CreateTournament";
import MainPage from "./components/MainPage";
import DisciplinesPage from "./Pages/DisciplinePages/DisciplineList";
import { ProtectedRoute } from "./utils/ProtectedRoute";
import { hasRole } from "./utils/auth";
import { useAuth } from "./api/AuthHook";
import TournamentEditPage from "./Pages/TournamentPages/TournamentEditPage";

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <Header />
        <div >
          <Container className="py-4 d-flex flex-column min-vh-100">
            <Routes>
              <Route path="/" element={<MainPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/login" element={<LoginForm />} />
              <Route path="/news" element={<NewsPage />} />
              <Route path="/news/:id" element={<NewsDetails />} />
              <Route path="/news/create" element={<NewsCreatePage />} />
              <Route path="/profile" element={<ProfilePage />} />
              <Route path="/users/:id" element={<UserDetailsAdmin />} />
              <Route path="/matches" element={<MatchesPage />} />
              <Route path="/matches/:id" element={<MatchDetails />} />
              <Route path="/tournaments" element={<TournamentList />} />
              <Route path="/tournaments/:id" element={<TournamentDetails />} />
              <Route path="/tournaments/update/:id" element={<TournamentEditPage />} />
              <Route path="/tournaments/:id/add-participant" element={<AddParticipantPage />} />
              <Route path="/tournaments/:tournamentid/edit-participant/:id" element={<ParticipantEdit />} />
              <Route path="/tournaments/create" element={<CreateTournament />} />
              <Route element={<ProtectedRoute isAllowed={hasRole('admin')} />}>
                <Route path="/admin/users" element={<UserList />} />
                <Route path="/admin/disciplines" element={<DisciplinesPage />} />
              </Route>
            </Routes>
          </Container>
        </div>
        <Footer />
      </Router>
    </AuthProvider>
  );
};

export default App;
