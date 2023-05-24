import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { BrowserRouter, Route, Routes, Navigate } from 'react-router-dom'
import NavBar from './components/NavBar/NavBar';
import Login from './components/Login/Login';
import Register from './components/Register/Register';
import MyListTable from './components/Mylist/Mylist';
import Movie from './components/Movie/Movie';
import { RightSideBar } from './components/RightSideBar/RightSideBar';
import Users from './components/Users/Users';
import Container from './components/Container/Container';
import BasicTabs from './components/Tabs/Tabs';
import TempContainer from './components/Container/TempContainer';
import Chat from './components/Chat/Chat';
import ChatContainer from './components/Container/ChatContainer';
import RecomContainer from './components/Container/RecomContainer';
import ProfileContainer from './components/Container/ProfileContainer';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import ResetPassword from './components/ResetPassword/ResetPassword'; // Import the ResetPassword component
import LoginSide from './components/Login/LoginSide';
import RegisterSide from './components/Register/RegisterSide';
import NotFound from './components/NotFound/NotFound';
import ProtectedRoutes from './components/ProtectedRoute/ProtectedRoute';
import ProtectedWrapper from './components/ProtectedRoute/ProtectedWrapper';
import ResetPasswordEmailSide from './components/ResetPassword/ResetPasswordEmailSide';
import ResetPasswordNewCurrentPasswordSide from './components/ResetPassword/ResetPasswordNewCurrentPasswordSide';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#FFFFFF',
    },
    secondary: {
      main: '#f50057',
    }
  },
  typography: {
    fontFamily: [
      'Roboto',
      '-apple-system',
      'BlinkMacSystemFont',
      'Segoe UI',
      'Helvetica',
      'Arial',
      'sans-serif',
      'Apple Color Emoji',
      'Segoe UI Emoji',
      'Segoe UI Symbol',
    ].join(','),
    fontSize: 14,
    fontWeightRegular: 400,
    fontWeightMedium: 500,
    fontWeightBold: 700,
    h1: {
      fontSize: '2.5rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    h2: {
      fontSize: '2rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    h3: {
      fontSize: '1.5rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    h4: {
      fontSize: '1.2rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    h5: {
      fontSize: '1rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    h6: {
      fontSize: '0.875rem',
      fontWeight: 700,
      lineHeight: 1.2,
    },
    subtitle1: {
      fontSize: '1rem',
      fontWeight: 500,
      lineHeight: 1.2,
    },
    subtitle2: {
      fontSize: '0.875rem',
      fontWeight: 500,
      lineHeight: 1.2,
    },
    body1: {
      fontSize: '1rem',
      fontWeight: 400,
      lineHeight: 1.5,
    },
    body2: {
      fontSize: '0.875rem',
      fontWeight: 400,
      lineHeight: 1.5,
    },
    button: {
      fontSize: '0.875rem',
      fontWeight: 500,
      lineHeight: 1.5,
      textTransform: 'uppercase',
    },
  }
});


  const shouldRenderNavbar = () => {
    const currentPath = window.location.pathname;
    return currentPath !== '/login' && currentPath !== '/register' && currentPath !== '/not-found' && currentPath !==  '/reset-password' && currentPath !==  '/new-password';;
  };

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(

  <BrowserRouter>
   <ThemeProvider theme={theme}>
    {shouldRenderNavbar() && <BasicTabs />}
    <Routes>
  <Route path="/" element={<Navigate replace to="/home" />} />
  <Route path="/login" element={<LoginSide />} />
  <Route path="/register" element={<RegisterSide />} />
  <Route path="/reset-password" element={<ResetPasswordEmailSide />} />
  <Route path="/new-password" element={<ResetPasswordNewCurrentPasswordSide />} />
  <Route path="/not-found" element={<NotFound />} />
  
  <Route
    path="/*"
    element={
      <ProtectedRoutes>
        <ProtectedWrapper />
      </ProtectedRoutes>
    }
  >
   <Route path="home" element={<Container />} />
    <Route path="movies/:id" element={<TempContainer />} />
    <Route path="movies" element={<TempContainer />} />
    <Route path="movie/:id" element={<Movie />} />
    <Route path="recommendations/:id1?/:id2?" element={<RecomContainer />} />
    <Route path="chat/:id?" element={<ChatContainer />} />
    <Route path="profile/:userName?" element={<ProfileContainer />} />
    <Route path="profile" element={<ProfileContainer />} />
    <Route path="list/profile/:userName?" element={<ProfileContainer />} />
    <Route path="list/profile" element={<ProfileContainer />} />
    <Route path="*" element={<Navigate to="/not-found" />} />
  </Route>
</Routes>
    </ThemeProvider>
  </BrowserRouter>
);

//<Route path="/login" element={<Login />} />
     // <Route path="/register" element={<Register />} />
// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
