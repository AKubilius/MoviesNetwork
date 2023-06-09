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

const SUCCESS_MESSAGE = 'Slaptažodžio priminimo el. laiškas išsiųstas';

export default function ResetPasswordEmailSide() {
  const [isEmailSent] = useState(false);

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
            Slaptažodžio priminimas
          </Typography>
          <Box component='form' onSubmit={console.log} sx={{ mt: 1 }}>

            <TextField
              margin='normal'
              required
              fullWidth
              id='email'
              label='El. Paštas'
              name='email'
            />

            {isEmailSent && (
              <Alert severity='success'>{SUCCESS_MESSAGE}</Alert>
            )}

            <Button
              type='submit'
              fullWidth
              variant='contained'
              sx={{ mt: 3, mb: 2 }}
            >
              Priminti slaptažodį
            </Button>
            <Grid container>
              <Grid item xs>
                <Link href='/login' variant='body2'>
                  Prisiminėte slaptažodį? Prisijunkite
                </Link>
              </Grid>
            </Grid>
          </Box>
        </Box>
      </Grid>
    </Grid>
  );
}