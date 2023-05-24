import {
    Alert,
    Avatar,
    Box,
    Button,
    Grid,
    Link,
    TextField,
    Typography,
  } from "@mui/material";
  import AppRegistrationIcon from "@mui/icons-material/AppRegistration";
  import { useState } from "react";
  import FormContainer from "../Form/Form";
  import axios from "axios";
  import {api,authConfig} from "../Api/Api";
  
  const ErrorMessages = {
    EMPTY_FIELDS: "Visi laukai yra privalomi!",
    MISSMATCHING_PASSWORDS: "Slaptažodžiai turi sutapti",
    INCORRECT_PASSWORD_FORMAT:
      "Slaptažodį turi sudaryt bent 8 simboliai, įskaitant skaitmenį, didžiąją raidę ir simbolį",
    INCORRECT_EMAIL_FORMAT: "Netinkamas El. Paštas",
    UNEXPECTED_ERROR: "Įvyko netikėta klaida, bandykite vėliau",
  };
  
  const SUCCESS_MESSAGE = "Registracija patvirtinta";
  
  const Registration: React.FC = () => {
    const [errorMessage, setErrorMessage] = useState("");
    const [isRegistrationSuccessful, setIsRegistrationSuccessful] =
      useState(false);
  
    const handleOnSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      setErrorMessage("");
      const formData = new FormData(e.currentTarget);
      const registrationData = {
        userName: formData.get("userName"),
        email: formData.get("email"),
        password: formData.get("password"),
      };
      
      try {
        await api.post("/api/register", registrationData);
        setIsRegistrationSuccessful(true);
      } catch (error: any) {
        setErrorMessage(ErrorMessages.UNEXPECTED_ERROR);
      }
    };
  
    return (
      <FormContainer>
        <Avatar sx={{ m: 1, bgcolor: "secondary.main" }}>
          <AppRegistrationIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Registracija
        </Typography>
        <Box component="form" onSubmit={handleOnSubmit} sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="userName"
            label="Vardas"
            name="userName"
            autoFocus
          />

          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="El. Paštas"
            name="email"
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
          {isRegistrationSuccessful && (
            <Alert severity="success">{SUCCESS_MESSAGE}</Alert>
          )}
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            
            Registruotis
          </Button>
          <Grid container justifyContent="center">
            <Grid item>
              <Link href="/login" variant="body2">
                Turite paskyrą? Prisijunkite
              </Link>
            </Grid>
          </Grid>
        </Box>
      </FormContainer>
    );
  };
  
  export default Registration;