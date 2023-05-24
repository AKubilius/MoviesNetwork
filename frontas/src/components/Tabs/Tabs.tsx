import * as React from 'react';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar/Toolbar';
import Container from '@mui/material/Container';
import AppBar from '@mui/material/AppBar/AppBar';
import { Link as RouterLink , useParams,NavLink ,useLocation  } from 'react-router-dom';
import Link from '@mui/material/Link';
import Avatar from '@mui/material/Avatar/Avatar';
import { NavBarRequests } from './NavBarRequests';
import { NavBarNotifications } from './NavBarNotifications';
import { useEffect, useState } from 'react';
import Loading from './Loading';
import { logout } from '../Api/Api';
import Button from '@mui/material/Button/Button';
import axios from 'axios';

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    'aria-controls': `simple-tabpanel-${index}`,
  };
}

const handleLogout = () => {
  logout();
};

export default function BasicTabs() {
  const [loading, setLoading] = useState(false);
  const location = useLocation();

  const [image,setImage] = useState<any>([]);

  const token = `Bearer ${sessionStorage.getItem("token")}`
  const getUserImage = async () => {
    const { data } = await axios.get(`https://localhost:7019/User/currentImage`,
      {
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
          Authorization: token
        },
      })
    setImage(data)
  }
  useEffect(() => {
    getUserImage()
  }, [])


  const logged = sessionStorage.getItem("token");

  const [value, setValue] = useState(() => {
    if (location.pathname.startsWith('/home')) {
      return 0;
    } else if (location.pathname.startsWith('/movie')) {
      return 1;
    } else if (location.pathname.startsWith('/chat')) {
      return 2;
    } else if (location.pathname.startsWith('/recommendations')) {
      return 3;
    } else {
      return 0;
    }
  });


  useEffect(() => {
    if (location.pathname.startsWith('/home')) {
      setValue(0);
    } else if (location.pathname.startsWith('/movie')) {
      setValue(1);
    } else if (location.pathname.startsWith('/chat')) {
      setValue(2);
    } else if (location.pathname.startsWith('/recommendations')) {
      setValue(3);
    } else {
      setValue(0);
    }
  }, [location.pathname]);
 
  useEffect(() => {
    const timer = setTimeout(() => {
      setLoading(false);
    }, 500); // Adjust the delay as needed

    return () => clearTimeout(timer);
  }, [location.pathname]);

  const handleChange = (event: React.SyntheticEvent, newValue: number) => {
    setLoading(true);
    setValue(newValue);
  };

  return (
    <>
      {loading ? <Loading /> :
        <AppBar position="fixed" sx={{ top: -10 }}>
          <Container maxWidth="xl" >
            <Toolbar sx={{ justifyContent: 'space-between', alignContent:'end' }} >
              <Box sx={{ display: 'flex', flexDirection: 'row' }}>
                <h3>CineConnect</h3>
              </Box>

              <Box sx={{ borderBottom: 1, borderColor: 'divider', alignSelf: 'flex-end' }}>
              <Tabs value={value} onChange={handleChange}>
                <NavLink to="/home" style={{ textDecoration: 'none' }}>
                  <Tab sx={{color: 'white'}} disableRipple label="Srautas" {...a11yProps(0)} />
                </NavLink>
                <NavLink to="/movies" style={{ textDecoration: 'none' }}>
                  <Tab sx={{color: 'white'}} disableRipple label="Medija" {...a11yProps(1)} />
                </NavLink>
                <NavLink to="/chat" style={{ textDecoration: 'none' }}>
                  <Tab sx={{color: 'white'}} disableRipple label="Žinutės" {...a11yProps(2)} />
                </NavLink>
                <NavLink to="/recommendations" style={{ textDecoration: 'none' }}>
                  <Tab sx={{color: 'white'}} disableRipple label="Rekomendacijos" {...a11yProps(2)} />
                </NavLink>
              </Tabs></Box>
{logged ? (<Box sx={{ display: 'flex', flexDirection: 'row', alignContent: 'center', justifyContent: 'center', marginTop: 1 }}>
                <NavBarRequests />
                <NavBarNotifications />
                <Link to="/profile" component={RouterLink}>
                <Avatar src={`data:image/jpeg;base64,${image}`} ></Avatar>
                </Link>
                <Button onClick={handleLogout}>
      Atsijungti
    </Button>
              </Box>) : (<Box>
                <Link  to={'/login'} component={RouterLink} >Prisijungti</Link>
                <Link to={'/register' } component={RouterLink}>Registracija</Link>

              </Box>) }

            </Toolbar>
          </Container>
        </AppBar>
      }
    </>
  );
}