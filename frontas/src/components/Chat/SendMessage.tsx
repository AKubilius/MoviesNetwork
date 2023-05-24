import Box from '@mui/material/Box/Box';
import Button from '@mui/material/Button/Button';
import TextField from '@mui/material/TextField/TextField';
import React, { FC } from 'react';
import SendIcon from '@mui/icons-material/Send';
import * as signalR from '@microsoft/signalr';
import styled from '@mui/material/styles/styled';
import { useParams } from 'react-router-dom';


const CustomTextField = styled(TextField)({
  borderColor: 'black',
  '& .MuiOutlinedInput-root': {
    '& fieldset': {
      borderColor: 'black', 
    },
    '&:hover fieldset': {
      borderColor: '#121212', 
    }
    
  },
   '&.MuiOutlinedInput-notchedOutline': {
    borderColor: 'red', 
  },
  '& .MuiInputBase-input': {
    color: 'black', 
  },
});

interface SendMessageProps {
  input: string;
  setInput: (value: string) => void;
  sendMessage: () => void;
  connection: signalR.HubConnection | null;
  roomID: string | undefined;
}

const SendMessage: FC<SendMessageProps> = ({ input, setInput, sendMessage, connection, roomID }) => {
  const userName = `${sessionStorage.getItem("name")}`;
 
  const handleSendMessage = async () => {
    if (input && connection && roomID && connection.state === signalR.HubConnectionState.Connected) {
      await connection.send('SendMessageToRoom', roomID, userName, input);
      setInput('');
    }
  };

  if(!roomID)
  {
    return <div></div>
  }


  return (
  <Box sx={{
    position: 'sticky',
    display:'flex',
    flexDirection:'row',
    alignItems: 'center',
    backgroundColor: '#E8F4FD',

}}>
     <CustomTextField
  fullWidth
  id="outlined-multiline-static"
  placeholder={'Aa'}
  sx={{ margin:2, overflow: 'hidden',borderColor:'black'}}
  inputProps={{ style: { overflow: 'hidden' } }}
  value={input}
  onChange={(e) => setInput(e.target.value)}
  onKeyPress={(e) => {
    if (e.key === 'Enter') {
      sendMessage();
    }
  }}
/>
    <Button
        variant="contained"
        color="primary"
        onClick={handleSendMessage}
        sx={{
            borderRadius: 5,
            height:'40px',
            fontWeight: 'bold',
          
        }}
        endIcon={<SendIcon />}
    >
    </Button>
</Box>
 );
};
export default SendMessage;