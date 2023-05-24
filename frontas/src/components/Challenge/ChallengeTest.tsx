import * as React from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import Modal from '@mui/material/Modal';
import Challenge from './Challenge';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import TextField from '@mui/material/TextField';
import { useState } from 'react';

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  p: 4,
};

interface Challenges
{
  name:string;
  count:number;
}
interface userChallenge {
  progress:number;
  completed:boolean;
  challenge:Challenges;
}

interface UserChallengeProps {
    userChallenge: userChallenge[];
  }

  const h4Style = {display:'flex', justifyContent:'center'}
  const line = { borderTop: '1px solid #ccc', margin: '16px 0' }

  export default function ChallengesTest() {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);


  const [fields, setFields] = useState(['', '', '']);

  const handleChange = (index: number, event: React.ChangeEvent<HTMLElement>) => {
    const newFields = [...fields];
    const target = event.target as HTMLInputElement;
    newFields[index] = target.value;
    setFields(newFields);
  };

  const addField = () => {
    setFields([...fields, '']);
  };

  return (
    <div style={{display:'flex',justifyContent:'end'}}>
      <Button onClick={handleOpen}>Visi iššūkiai</Button>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={style}>
        {fields.map((field, index) => (
        <TextField
          key={index}
          label={`Field ${index + 1}`}
          value={field}
          onChange={(event) => handleChange(index, event)}
          fullWidth
          margin="normal"
        />
      ))}
      <Button variant="contained" color="primary" onClick={addField} sx={{ mt: 2 }}>
        Add Field
      </Button>
        </Box>
      </Modal>
    </div>
  );
}