import { useState } from 'react';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import { api } from '../Api/Api';
import axios from 'axios';
import Alert from '@mui/material/Alert';
import poster from './Poster.png';
import AppRegistrationIcon from '@mui/icons-material/AppRegistration';
import { useLocation } from 'react-router-dom';

const SUCCESS_MESSAGE = 'Slaptažodžio priminimo el. laiškas išsiųstas';

export default function ResetPasswordNewCurrentPasswordSide() {
  const [isPaswordChangedSuccessfully] = useState(false);
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
    <Grid container sx={{ height: '100vh' }}>
      <CssBaseline />
      <Grid
        item
        xs={false}
        sm={4}
        md={7}
        sx={{
          backgroundImage: `url(${poster})`,
          backgroundRepeat: 'no-repeat',
          backgroundColor: (t) =>
            t.palette.mode === 'light'
              ? t.palette.grey[50]
              : t.palette.grey[900],
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      />
      <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6} square>
        <Box
          sx={{
            my: 8,
            mx: 4,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
            <AppRegistrationIcon />
          </Avatar>
          <Typography component='h1' variant='h5'>
            Slaptažodžio pakeitimas
          </Typography>
          <Box component='form' onSubmit={handleSubmit} sx={{ mt: 1 }}>

          <TextField
              margin='normal'
              required
              fullWidth
              name='password'
              label='Naujas slaptažodis'
              type='password'
              id='password'
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
            />

            {isPaswordChangedSuccessfully && (
              <Alert severity='success'>{SUCCESS_MESSAGE}</Alert>
            )}

            <Button
              type='submit'
              fullWidth
              variant='contained'
              sx={{ mt: 3, mb: 2 }}
            >
              Pakeisti slaptažodį
            </Button>
            <Grid container>
              <Grid item xs>
                <Link href='/home' variant='body2'>
                  Grįžti į pradžią
                </Link>
              </Grid>
            </Grid>
          </Box>
        </Box>
      </Grid>
    </Grid>
  );
}