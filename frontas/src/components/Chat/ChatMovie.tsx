import Box from '@mui/material/Box/Box'
import Button from '@mui/material/Button/Button'
import React, { useEffect, useState } from 'react'
import CheckIcon from '@mui/icons-material/Check';
import DoDisturbIcon from '@mui/icons-material/DoDisturb';
import RemoveCircleIcon from '@mui/icons-material/RemoveCircle';
import dayjs from 'dayjs';
import { getRequest, makePutRequest } from '../Api/Api';

 

  interface Request {
    status:number;
    watchingDate:Date;
  }

  const style4 ={
margin:0
  }
  const style3 ={

  }
export const ChatMovie = (movie: any) => 
{
  
  const handleClickAccept = async (Id: any) => {
    const { data } = await makePutRequest(`https://localhost:7019/api/WatchingRequest/accept/${Id}`, { });
    if (requests) {
      setRequests({ ...requests, status: 1 }); 
    }
    return data;
  };
  const handleClickDecline = async (Id: any) => {
    const { data } = await makePutRequest(`https://localhost:7019/api/WatchingRequest/decline/${Id}`, { });
    if (requests) {
      setRequests({ ...requests, status: 2 }); 
    }
    return data;
  };
 

  const [requests, setRequests] = useState<Request>();

  const getBoxClassName = (status: number) => {
    switch (status) {
      case 0:
        return 'message-content';
      case 1:
        return 'message-content Accept';
      case 2:
        return 'message-content Decline';
      default:
        return 'message-content';
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      const data = await getRequest('https://localhost:7019/api/WatchingRequest/chat/', movie.id );
      setRequests(data);
    };
    fetchData();
    
  }, []);

  return (

    <div className={getBoxClassName(requests?.status ?? 0)}>
    <h2 style={{display:'flex',justifyContent:'center',margin:0}}>
    {requests?.status=== 1 && "Pakvietimas Priimtas"}
  {requests?.status === 2 && "Pakvietimas Atšauktas"}
  {requests?.status !== 1 && requests?.status !== 2 && "Pakvietimas žiūrėti"}</h2>
    
    {requests ?  <h4 style={style4}>{dayjs(requests.watchingDate).format('YYYY-MM-DD HH:mm')}</h4> : '' }
    <Box
      component="img"
      sx={{
        margin: 1,
        borderRadius: 2,
        height: 1,
        width: '40%',
      }}
      alt="Movie"
      src={`${movie.Url}`} />
 <h2 style={style4}>{movie.title} </h2> 
      {!(requests?.status === 1 || requests?.status === 2 )&& (
      <Box sx={{
        width:1,
        display:'flex',
        flexDirection:'row',
        justifyContent:'space-around'
      }}> 
    {movie.sender !== movie.username && (
      <>
        <Button
          variant="contained"
          size="large"
          sx={{
            borderRadius: 5,
            margin: 1,
          }}
          startIcon={<CheckIcon sx={{
            color: 'green'
          }} />}
          onClick={() => handleClickAccept(movie.id)} >
          Priimti
        </Button>
        <Button
          variant="contained"
          size="large"
          sx={{
            borderRadius: 5,
            margin: 1,
          }}
          startIcon={<RemoveCircleIcon sx={{
            color: 'red'
          }} />}
          onClick={() => handleClickDecline(movie.id)}>
          Atmesti
        </Button>
      </>
    )}
    </Box>
    )}
    {/* Render additional movie information, such as image and title, when available */}
  </div>
  )
}
