import {
    Box,
    Container,
    CssBaseline,
  } from "@mui/material";
  
  interface FormContainerProps {
    children: React.ReactNode;
  }
  
  const Form: React.FC<FormContainerProps> = ({ children }) => {
  
    return (
        <Container component="main" maxWidth="xs">
          <CssBaseline />
          <Box
            sx={{
              marginTop: 8,
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
            }}
          >
            {children}
          </Box>
        </Container>
    );
  };
  
  export default Form;