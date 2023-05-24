import * as React from 'react';
import { DemoContainer } from '@mui/x-date-pickers/internals/demo';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { useState } from 'react';
import { PickerChangeHandlerContext } from '@mui/x-date-pickers/internals/hooks/usePicker/usePickerValue.types';
import { DateTimeValidationError } from '@mui/x-date-pickers/models/validation';
import dayjs, { Dayjs } from 'dayjs';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { AdapterMoment } from '@mui/x-date-pickers/AdapterMoment';

interface BasicDateTimePickerProps {
    onDateChange: (date: Dayjs| null) => void;
  }
  
  export default function BasicDateTimePicker({ onDateChange }: BasicDateTimePickerProps) {

    const [selectedDate, setSelectedDate] = React.useState<Dayjs | null>(dayjs('2023-05-10T15:30'));    
    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DemoContainer components={['DateTimePicker']}>
                <DateTimePicker
                    value={selectedDate}
                    onChange={(newValue) => onDateChange(newValue)}
                    ampm={false}
                    label="Pasirinkite laiką" />
            </DemoContainer>
        </LocalizationProvider>
    );
}