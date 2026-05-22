import { useState, useEffect } from "react";
import { Link, useNavigate } from 'react-router-dom';
import api from "../api/axios";

export default function StudentList() {
    const [students, setStudents] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
const navigate = useNavigate(); // ✅ hook for navigation
    const fetchStudents = async () => {
        try {
            const response = await api.get('/students');    
            setStudents(response.data);
            setErrorMessage('');
        } catch (error) {
            const status = error?.response?.status;
            const details = error?.response?.data;
            const message = status
                ? `Unable to load students (HTTP ${status}). ${details || ''}`.trim()
                : 'Unable to load students. Please try again in a moment.';
            setErrorMessage(message);
            console.error('Error fetching students:', error);
        }
    };

    useEffect(() => {
        fetchStudents();
    }, []);

    const handleDelete = async (id) => {
        try {
            await api.delete(`/students/${id}`);
            fetchStudents();
        } catch (error) {
            console.error('Error deleting student:', error);
        }
    };

    return (
       <div>
        <h2>Students</h2>
        {errorMessage && <p style={{ color: 'crimson' }}>{errorMessage}</p>}
      <Link to="/StudentForm">Add Student</Link>
      <ul>
        {students.map(s => (
          <li key={s.id}>
            {s.name} ({s.email}) - Stream {s.streamId}
            <button onClick={() => navigate(`/StudentForm/edit/${s.id}`)}>Edit</button>
            <button onClick={() => handleDelete(s.id)}>Soft Delete</button>
          </li>
        ))}
      </ul>
    </div>
    );
}