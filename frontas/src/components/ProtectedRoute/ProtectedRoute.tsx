import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

interface ProtectedRoutesProps {
  children: React.ReactNode;
}
const ProtectedRoutes: React.FC<ProtectedRoutesProps> = ({ children }) => {
  const navigate = useNavigate();
  const isAuthenticated = sessionStorage.getItem('token') ? true : false;
  console.log('isAuthenticated:', isAuthenticated); // Add this line
  useEffect(() => {
    if (!isAuthenticated) {
      window.location.href = '/login';
    }
  }, [isAuthenticated, navigate]);

  return <>{isAuthenticated && children}</>;
};

export default ProtectedRoutes;