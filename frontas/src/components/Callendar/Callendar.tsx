import * as React from 'react';
import axios from 'axios';
import dayjs, { Dayjs } from 'dayjs';
import Badge from '@mui/material/Badge';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { PickersDay, PickersDayProps } from '@mui/x-date-pickers/PickersDay';
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { DayCalendarSkeleton } from '@mui/x-date-pickers/DayCalendarSkeleton';
import { getRequest } from '../Api/Api';

import 'dayjs/locale/lt'; 

dayjs.locale('lt'); 

const token = `Bearer ${sessionStorage.getItem("token")}`;

function ServerDay(props: PickersDayProps<Dayjs> & { highlightedDays?: number[] }) {
  const { highlightedDays = [], day, outsideCurrentMonth, ...other } = props;

  const isSelected = (day: number, highlightedDays: number[]) => {
    return highlightedDays.includes(day);
  };
 
  const backgroundColor = isSelected(props.day.date(),highlightedDays) ? '#66bb6a' : undefined;

  return (
    <Badge
      key={props.day.toString()}
      overlap="circular"
      badgeContent={isSelected(props.day.date(),highlightedDays) ? '✔️' : undefined}
    >
      <PickersDay
        {...other}
        outsideCurrentMonth={outsideCurrentMonth}
        day={day}
        sx={{
          backgroundColor,
          '&:hover': {
            backgroundColor,
          },
        }}
      />
    </Badge>
  );
}

interface ChallengesBoxProps {
  userName: string | undefined;
}
  
  const DateCalendarServerRequest: React.FC<ChallengesBoxProps> = ({ userName }) => {
  const [isLoading, setIsLoading] = React.useState(false);
  const [highlightedDays, setHighlightedDays] = React.useState<number[]>([]);
  const [currentDate, setCurrentDate] = React.useState<Dayjs>(dayjs());

  const fetchWatchingRequests = async (date: Dayjs) => {
    setIsLoading(true);
    try {
      const response = await getRequest('https://localhost:7019/api/WatchingRequest/', userName ? userName : '');
       
      const filteredRequests = response.filter((request: any) => request.status === 1);
     
      const acceptedDates = response
  .filter((request: any) => request.status === 1)
  .map((request: any) => dayjs(request.watchingDate).startOf('day'));
  
        const daysToHighlight = acceptedDates
        .filter((acceptedDate: Dayjs) => acceptedDate.month() === date.month())
        .map((acceptedDate: Dayjs) => acceptedDate.date());
  
      setHighlightedDays(daysToHighlight);
      setIsLoading(false);
    } catch (error) {
      
      setIsLoading(false);
    }
  };

  
  React.useEffect(() => {
    fetchWatchingRequests(currentDate);
  }, [currentDate]);

  const handleMonthChange = (date: Dayjs) => {
    setCurrentDate(date);
  };

  return (
    <LocalizationProvider  dateAdapter={AdapterDayjs}>
      <DateCalendar
        defaultValue={currentDate}
        loading={isLoading}
        onMonthChange={handleMonthChange}
        renderLoading={() => <DayCalendarSkeleton />}
        slots={{
          day: ServerDay,
        }}
        slotProps={{
          day: {
            highlightedDays,
          } as any,
        }}
      />
    </LocalizationProvider>
  );
}
export default DateCalendarServerRequest