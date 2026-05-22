import React, { useState, useEffect } from "react";
import api from "../api/axios";
import { useNavigate } from 'react-router-dom';

// import AdmissionForm from '../components/AdmissionForm';
// export default function AdmissionPage() { return <AdmissionForm />; }

export default function AdmissionForm() {
   const [studentId, setStudentId] = useState('');
   const [feesPaid, setFeesPaid] = useState('');
  const [students, setStudents] = useState([]);
  const [admissions, setAdmissions] = useState([]);
  const [isAdmissionsLoading, setIsAdmissionsLoading] = useState(false);
  const [admissionsError, setAdmissionsError] = useState('');
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

     useEffect(() => {
       if (!studentId) {
         setAdmissions([]);
         setAdmissionsError('');
         return;
       }

       const fetchAdmissionsByStudent = async () => {
         setIsAdmissionsLoading(true);
         setAdmissionsError('');

         try {
           const response = await api.get(`/admissions/bystudent/${studentId}`);
           const payload = response?.data;
           const normalized = Array.isArray(payload) ? payload : payload ? [payload] : [];
           setAdmissions(normalized);
         } catch (error) {
           if (error?.response?.status === 404) {
             setAdmissions([]);
           } else {
             console.error('Error fetching admissions:', error);
             setAdmissionsError('Failed to load admissions for selected student.');
           }
         } finally {
           setIsAdmissionsLoading(false);
         }
       };

       fetchAdmissionsByStudent();
     }, [studentId]);

   const handleSubmit = async (e) => {
       e.preventDefault();  
        await api.post(`/admissions/confirm?studentId=${studentId}&feesPaid=${feesPaid}`);
        alert('Admission confirmed!');

      if (studentId) {
        const response = await api.get(`/admissions/bystudent/${studentId}`);
        const payload = response?.data;
        const normalized = Array.isArray(payload) ? payload : payload ? [payload] : [];
        setAdmissions(normalized);
      }

      navigate('/StudentForm');
  };

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <select value={studentId} onChange={(e) => setStudentId(e.target.value)}>
          <option value="">Select Student</option>
          {students.map((s) => (
            <option key={s.id} value={s.id}>
              {s.name} ({s.email})
            </option>
          ))}
        </select>
        <input value={feesPaid} onChange={(e) => setFeesPaid(e.target.value)} placeholder="Fees Paid" />
        <button type="submit">Confirm Admission</button>
      </form>

      <h3>Admissions</h3>
      {isAdmissionsLoading && <p>Loading admissions...</p>}
      {admissionsError && <p>{admissionsError}</p>}
      {!isAdmissionsLoading && !admissionsError && studentId && admissions.length === 0 && (
        <p>No admissions found for selected student.</p>
      )}

      {!isAdmissionsLoading && !admissionsError && admissions.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Admission ID</th>
              <th>Student ID</th>
              <th>Fees Paid</th>
              <th>Admission Date</th>
              <th>Confirmed</th>
            </tr>
          </thead>
          <tbody>
            {admissions.map((admission) => (
              <tr key={admission.id}>
                <td>{admission.id}</td>
                <td>{admission.studentId}</td>
                <td>{admission.feesPaid}</td>
                <td>{admission.admissionDate ? new Date(admission.admissionDate).toLocaleString() : ''}</td>
                <td>{admission.isConfirmed ? 'Yes' : 'No'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}