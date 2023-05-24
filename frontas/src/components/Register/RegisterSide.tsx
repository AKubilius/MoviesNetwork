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

const ErrorMessages = {
  EMPTY_FIELDS: 'Visi laukai yra privalomi!',
  MISSMATCHING_PASSWORDS: 'Slaptažodžiai turi sutapti',
  INCORRECT_PASSWORD_FORMAT:
    'Slaptažodį turi sudaryt bent 8 simboliai, įskaitant skaitmenį, didžiąją raidę ir simbolį',
  INCORRECT_EMAIL_FORMAT: 'Netinkamas El. Paštas',
  UNEXPECTED_ERROR: 'Įvyko netikėta klaida, bandykite vėliau',
};

const SUCCESS_MESSAGE = 'Registracija patvirtinta';

export default function RegistrationSide() {
  const [errorMessage, setErrorMessage] = useState('');
  const [isRegistrationSuccessful, setIsRegistrationSuccessful] =
    useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setErrorMessage('');
    const formData = new FormData(e.currentTarget);
    const registrationData = {
      userName: formData.get('userName'),
      email: formData.get('email'),
      password: formData.get('password'),
    };

    try {
      await api.post('/api/register', registrationData);
      setIsRegistrationSuccessful(true);
      window.location.href = "/home";
    } catch (error: any) {
      setErrorMessage(ErrorMessages.UNEXPECTED_ERROR);
    }
  };

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
            Registracija
          </Typography>
          <Box component='form' onSubmit={handleSubmit} sx={{ mt: 1 }}>
            <TextField
              margin='normal'
              required
              fullWidth
              id='userName'
              label='Vardas'
              name='userName'
              autoFocus
            />

            <TextField
              margin='normal'
              required
              fullWidth
              id='email'
              label='El. Paštas'
              name='email'
            />
            
            <TextField
              margin='normal'
              required
              fullWidth
              name='password'
              label='Slaptažodis'
              type='password'
              id='password'
            />
            
            {errorMessage && <Alert severity='error'>{errorMessage}</Alert>}
            {isRegistrationSuccessful && (
              <Alert severity='success'>{SUCCESS_MESSAGE}</Alert>
            )}
            <Button
              type='submit'
              fullWidth
              variant='contained'
              sx={{ mt: 3, mb: 2 }}
            >
              Registruotis
            </Button>
            <Grid container>
              <Grid item xs>
                <Link href='/login' variant='body2'>
                  Turite paskyrą? Prisijunkite
                </Link>
              </Grid>
            </Grid>
          </Box>
        </Box>
      </Grid>
    </Grid>
  );
}
