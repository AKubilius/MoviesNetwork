import { Theme, ThemeProvider } from "@mui/material";
import { ReactNode } from "react";

type LoginLayoutProps = {
  children: ReactNode;
  theme: Theme;
};

const LoginLayout = ({ children, theme }: LoginLayoutProps) => {
  return <ThemeProvider theme={theme}>{children}</ThemeProvider>;
};

export default LoginLayout;
