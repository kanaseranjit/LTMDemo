import { useState, useEffect } from "react";
import api from "../api/axios";
import { useNavigate, useParams } from 'react-router-dom';

export default function StudentForm({existingStudent, onSuccess}) {
    const [student, setStudent] = useState(existingStudent || {name: '', age: '', emailID : '', stream: '' });
     const navigate = useNavigate(); // ✅ hook for navigation
     const { id } = useParams(); // get student id from route

// If editing, fetch student details
  useEffect(() => {
    const fetchStudent = async () => {
      if (id) {
        try {
          const res = await api.get(`/students/${id}`);
          setStudent(res.data);
        } catch (error) {
          console.error("Error fetching student:", error);
        }
      }
    };
    fetchStudent();
  }, [id]);

  const handleChange = (e) => {
    setStudent({ ...student, [e.target.name]: e.target.value });
  };

 const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (id) {
        await api.put(`/students/${id}`, student);
      } else {
        await api.post('/students', student);
      }
      navigate('/studentsList'); // redirect back to list
    } catch (error) {
      console.error("Error saving student:", error);
      alert("Failed to save student");
    }
  };

return (
    <form onSubmit={handleSubmit}>
      <input name="name" value={student.name} onChange={handleChange} placeholder="Name" />
      <input name="age" value={student.age} onChange={handleChange} placeholder="Age" />
      <input name="email" value={student.email} onChange={handleChange} placeholder="Email" />
      <select name="streamId" value={student.streamId} onChange={handleChange}>
        <option value="">Select Stream</option>
        <option value="1">Arts</option>
        <option value="2">Commerce</option>
        <option value="3">Science</option>
        <option value="4">Engineering</option>
      </select>
      
      <button type="submit">{id ? 'Update' : 'Add'} Student</button>
    </form>
);



}