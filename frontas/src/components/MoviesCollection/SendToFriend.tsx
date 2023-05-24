import * as signalR from '@microsoft/signalr';
import React, { useEffect, useState } from "react";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogActions from "@mui/material/DialogActions";
import Button from "@mui/material/Button";
import axios from "axios";
import ReplyIcon from '@mui/icons-material/Reply';
import Box from "@mui/material/Box/Box";
import TextField from "@mui/material/TextField/TextField";
import DatePicker from '../Callendar/DatePicker';
import Modal from '@mui/material/Modal/Modal';
import Typography from "@mui/material/Typography";
import dayjs, { Dayjs } from 'dayjs';

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 600,
  bgcolor: 'background.paper',
  borderRadius: 5,
  boxShadow: 24,
  p: 4,
};

const SendToFriend = (info: any) => {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);
  const [selectedDate, setSelectedDate] = React.useState<Dayjs | null>(dayjs('2023-05-10T15:30'));    

  const userName = `${sessionStorage.getItem("name")}`;
  const token = `Bearer ${sessionStorage.getItem("token")}`;
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);


  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7019/chatHub/?paramName1=${userName}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();
  
    setConnection(newConnection);
  }, []);


  const sendMovieInfo = async (userId: string) => {
    const response = await axios.get(`https://localhost:7019/api/message/${userId}`, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    const roomId = response.data;
    if (connection) {
    try {

      if (connection.state !== signalR.HubConnectionState.Connected) {
        await connection.start();
      }
      if (roomId) {
        await connection.invoke("JoinRoom", roomId.toString());

        if (connection && connection.state === signalR.HubConnectionState.Connected) {
         
          await connection.send("SendRequestToRoom", roomId.toString(), userName, `${info.movieId}`, true, userId, selectedDate);
        }
      } else {
        console.log("No common room found between the users.");
      }
    } catch (error) {
      console.log("Error sending movie info:", error);
    }}
  };

  const handleDateChange = (date: Dayjs | null) => {
    setSelectedDate(date);
  };

  return (
    <div>
    <Button
      sx={{
        borderRadius: 5,
        margin: 1,
      }}
      startIcon={<ReplyIcon />}
      onClick={handleOpen}
    >
     Siūlyti
    </Button>
    <Modal
      open={open}
      onClose={handleClose}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
    >
      <Box sx={style}>
        <Typography id="modal-modal-title" variant="h6" component="h2">
          Siūlyti žiūrėti
        </Typography>
        <DatePicker onDateChange={handleDateChange} />
       
        <Box sx={{ display: "flex", flexDirection: "column" }}>
        <Box
                            component="img"
                            sx={{
                                marginTop:1,
                                marginBottom:1,
                                borderRadius: 2,
                                height: '90%',
                                width: 1,
                            }}
                            alt="Movie"
                            src={`https://image.tmdb.org/t/p/original${info.imgUrl}`}
                        />

        {info.friends?.map(
            (user: any | null, index: React.Key | null | undefined) => (
              <Box sx={{
                display: 'flex',
                flexDirection: 'row',
                margin: 1,
                justifyContent: 'space-between'
              }}>
                <p style={{ margin: 0 }}>{user.userName}</p>
                <Button variant="contained" onClick={() => sendMovieInfo(user.id)}> Siusti</Button>
              </Box>
            )
          )}
        </Box>
      </Box>
    </Modal>
  </div>
  );
};

export default SendToFriend;
