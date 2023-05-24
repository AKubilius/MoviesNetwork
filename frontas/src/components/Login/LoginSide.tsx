import { useState } from "react";
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import FormContainer from "../Form/Form";
import Link from '@mui/material/Link';
import Paper from '@mui/material/Paper';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import { api } from "../Api/Api";
import axios from "axios";
import Alert from "@mui/material/Alert";
import poster from './Poster.png';

const ErrorMessages = {
  EMPTY_FIELDS: "Visi laukai yra privalomi!",
  WRONG_LOGIN: "Neteisingi prisijungimo duomenys",
  UNEXPECTED_ERROR: "Įvyko nenumatyta klaida, bandykite vėliau",
};

export default function LoginSide() {
  const [errorMessage, setErrorMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsLoading(true);
    setErrorMessage("");
    const formData = new FormData(e.currentTarget);
    const loginData = {
      username: formData.get("userName"),
      password: formData.get("password"),
    };

    try {
      const { data } = await api.post("/api/login", loginData);
      sessionStorage.setItem("token", data.accessToken);
      sessionStorage.setItem("username", "client");
      sessionStorage.setItem("name", data.userName);
      sessionStorage.setItem("admin", data.admin);
      sessionStorage.setItem("image", data.image64);
      window.location.href = "/";
    } catch (error: any) {
      if (axios.isAxiosError(error)) {
        error?.response?.status === 400
          ? setErrorMessage(ErrorMessages.WRONG_LOGIN)
          : setErrorMessage(ErrorMessages.UNEXPECTED_ERROR);
      }
    }

    setIsLoading(false);
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
              t.palette.mode === 'light' ? t.palette.grey[50] : t.palette.grey[900],
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
              <LockOutlinedIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
              Prisijungimas
            </Typography>
            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="userName"
                label="Prisijungimo vardas"
                name="userName"
                autoFocus
              />
              <TextField
                margin="normal"
                required
                fullWidth
                name="password"
                label="Slaptažodis"
                type="password"
                id="password"
              />

              {errorMessage && <Alert severity="error">{errorMessage}</Alert>}
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
              >
                Prisijungti
              </Button>
              <Grid container >
                <Grid item xs>
                  <Link href="/reset-password" variant="body2">
                    Pamiršote Slaptažodį?
                  </Link>
                </Grid>
                <Grid item>
                  <Link href="/register" variant="body2">
                    {"Neturite paskyros? Užsiregistruokite"}
                  </Link>
                </Grid>
              </Grid>
            </Box>
          </Box>
        </Grid>
      </Grid>
  );
}