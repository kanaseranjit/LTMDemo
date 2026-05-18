import react,{useState,useEffect} from "react";
import {Link, useNavigate, useParams } from 'react-router-dom';
import api from "../api/axios";
 import StudentForm from "./StudentForm";

export default function StudentList() {
    const [students, setStudents] = useState([]);
    const [editingStudent, setEditingStudent] = useState(null);
const navigate = useNavigate(); // ✅ hook for navigation
    const fetchStudents = async () => {
        try {
            const response = await api.get('/students');    
            setStudents(response.data);
        } catch (error) {
            console.error('Error fetching students:', error);
        }
    };

    useEffect(() => {
        fetchStudents();
    }, []);

    const handleStudentSaved = () => {
        fetchStudents();
        setEditingStudent(null);
    };

    const handleEditStudent = (student) => {
        setEditingStudent(student);
    };

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
      {/* <h2>Students</h2>
      <StudentForm
        existingStudent={editingStudent}
        onSuccess={() => {
            fetchStudents();           // refresh the list
            setEditingStudent(null);   // reset editing state
        }}
        />        
        */}

        <h2>Students</h2>
      <Link to="/StudentForm/add">Add Student</Link>
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