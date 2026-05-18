import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';

import StudentsList from './Components/StudentList';
import AdmissionPage from './pages/AdmissionPage';
import StudentForm from './Components/StudentForm';

function App() {
  return (
    <Router>
      <nav>
        <Link to="/admission">Admission</Link>
         <br />
        <Link to="/studentsList">StudentsList</Link>        
      </nav>
      <Routes>
        <Route path="/StudentForm" element={<StudentForm />} />
        <Route path="/admission" element={<AdmissionPage />} />
        <Route path="/studentsList" element={<StudentsList />} />
        <Route path="/StudentForm/edit/:id" element={<StudentForm />} />
      </Routes>
    </Router>
  );
}

export default App;
