import { Box } from '@mui/material';
import React from 'react'
import LinearProgress, { LinearProgressProps } from '@mui/material/LinearProgress';

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

interface ChallengeComponentProps {
  userChallenge: userChallenge;
}

const style ={
  marginBottom:0
}

const linearStyle = {
  marginLeft:1,
  marginRight:1,
  flexGrow: 1,
  backgroundColor: 'rgba(0, 0, 0, 0.12)',
}
const Challenge: React.FC<ChallengeComponentProps> = ({ userChallenge }) => {
  
  return (
   <>
    <Box>
      <h3 style={style}> {userChallenge.challenge.name}</h3>
          <LinearProgress
            variant="determinate"
            value={userChallenge.progress /userChallenge.challenge.count * 100 }
            sx={linearStyle}
          />
          <span style={{ marginLeft: '1rem' }}>{userChallenge.progress} / {userChallenge.challenge.count}</span>
    </Box>
    </>
  )
}

export default Challenge