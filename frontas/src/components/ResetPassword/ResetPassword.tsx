import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';

function ResetPassword() {
  const [newPassword, setNewPassword] = useState('');
  const location = useLocation();


  async function confirmResetPassword(email: string | null, token: string | null, newPassword: string) {
    const response = await fetch("https://localhost:7019/api/confirm-reset-password", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Email: email,
        Token: token,
        NewPassword: newPassword,
      }),
    });
  
    if (response.ok) {
      const message = await response.text();
      console.log(message);
      window.location.href = "/home";
      // Handle success, e.g., show a success message or redirect the user
    } else {
      const error = await response.json();
      console.error(error);
      // Handle error, e.g., show an error message
    }
  }
  

  const handleSubmit = async (e: { preventDefault: () => void; }) => {
    e.preventDefault();
    confirmResetPassword(email, token, newPassword);
  };

  const searchParams = new URLSearchParams(location.search);
  const token = searchParams.get('token');
  const email = searchParams.get('email');

  return (
    <div>
      <h1>Reset Password</h1>
      <form onSubmit={handleSubmit}>
        <label>
          New Password:
          <input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
          />
        </label>
        <button type="submit">Reset Password</button>
      </form>
    </div>
  );
}

export default ResetPassword;