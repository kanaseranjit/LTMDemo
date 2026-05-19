import react,{useState, useEffect} from "react";
import api from "../api/axios";
import { useNavigate } from 'react-router-dom';

// import AdmissionForm from '../components/AdmissionForm';
// export default function AdmissionPage() { return <AdmissionForm />; }

export default function AdmissionForm() {
   const [studentId, setStudentId] = useState('');
   const [feesPaid, setFeesPaid] = useState('');
    const [students, setStudents] = useState([]);
  const navigate = useNavigate();

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

   const handleSubmit = async (e) => {
       e.preventDefault();  
        await api.post(`/admissions/confirm?studentId=${studentId}&feesPaid=${feesPaid}`);
        alert('Admission confirmed!');
        navigate('/StudentForm');
  };

  return (
    <form onSubmit={handleSubmit}>
         {/* Dropdown bound to students list */}
      <select value={studentId} onChange={e => setStudentId(e.target.value)}>
        <option value="">Select Student</option>
        {students.map(s => (
          <option key={s.id} value={s.id}>
            {s.name} ({s.email})
          </option>
        ))}
      </select>
     <input value={feesPaid} onChange={e => setFeesPaid(e.target.value)} placeholder="Fees Paid" />
      <button type="submit">Confirm Admission</button>
    </form>
  );
}