import { Box } from '@mui/material';
import React from 'react'
import LinearProgress, { LinearProgressProps } from '@mui/material/LinearProgress';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import Button from '@mui/material/Button';
import { makePostRequest } from '../Api/Api';

interface Challenges {

}
interface userChallenge {
  name: string;
  count: number;
  id: number;
}

interface ChallengeComponentProps {
  userChallenge: userChallenge;
}

const style = {
  marginBottom: 0
}
const linearStyle = {
  marginLeft: 1,
  marginRight: 1,
  flexGrow: 1,
  backgroundColor: 'white',
}
const AllChallenge: React.FC<ChallengeComponentProps> = ({ userChallenge }) => {
  console.log(userChallenge)

  async function joinChallenge() {
    const { data, status } = await makePostRequest('https://localhost:7019/api/Challenge/UserChallenge', { "ChallengeId": userChallenge.id });
    return data;
  }

  return (
    <>
      <Box>
        <h3 style={style}> {userChallenge.name}</h3>
        <Box sx={{
          display: 'flex',
          flexDirection: 'row',
          alignItems: 'center'
        }}>
          <LinearProgress
            variant="determinate"
            value={0}
            sx={linearStyle}
          />
          <Button onClick={joinChallenge} startIcon={<AddCircleIcon />} color={'primary'} />
        </Box>
        <span style={{ marginLeft: '1rem' }}>{0} / {userChallenge.count}</span>
      </Box>

    </>
  )
}

export default AllChallenge