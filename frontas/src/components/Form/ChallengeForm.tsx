import React, { useEffect, useState } from 'react';
import { Button, TextField, Grid, IconButton, InputLabel, Select, MenuItem } from '@mui/material';
import AddCircleIcon from '@mui/icons-material/AddCircle';
import RemoveCircleIcon from '@mui/icons-material/RemoveCircle';
import { SelectChangeEvent } from '@mui/material/Select';
import axios from 'axios';
import { makePostRequest } from '../Api/Api';


interface ConditionObject {
    Type: string;
    Value: string;
  }

const MyForm = () => {
    const [conditions, setConditions] = useState<ConditionObject[]>([{ Type: '', Value: '' }]);
    const [genres, setGenres] = useState([]);
    const [name, setName] = React.useState('');
    const [count, setCount] = React.useState(0);
    const [description, setDescription] = React.useState('');
    const fetchGenres = async () => {
        const response = await axios.get(
            `https://api.themoviedb.org/3/genre/movie/list?api_key=c9154564bc2ba422e5e0dede6af7f89b&language=lt-LT`
        );
        setGenres(response.data.genres);
    };

    useEffect(() => {
        fetchGenres();
    }, []);

    const handleInputChange = (
        event: SelectChangeEvent<string>,
        index: number,
        field: keyof ConditionObject
      ) => {
        const newConditions = [...conditions];
        newConditions[index][field] = event.target.value as ConditionObject[keyof ConditionObject];
        setConditions(newConditions);
      };

    const addCondition = () => {
        setConditions([...conditions, { Type: '', Value: '' }]);
    };

    const removeCondition = (index: number) => {
        setConditions(conditions.filter((_, i) => i !== index));
    };

    const handleConditionChange = (event: SelectChangeEvent<string>, index: number) => {
        const newConditions = [...conditions];
        newConditions[index].Type = event.target.value;
        setConditions(newConditions);
    };

    const handleValueChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, index: number) => {
        const updatedConditions = [...conditions];
        updatedConditions[index].Value = event.target.value;
        setConditions(updatedConditions);
    };


    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        
        const formData = {
          Name: name,
          Description: description,
          Count: count,
          Conditions: conditions,
        };

        try {
            const response = await makePostRequest('https://localhost:7019/api/Challenge', formData);
            if (response.data) {
              console.log(response.data);
            } else {
              console.error('Error submitting the form: No data in response', response);
            }
          } catch (error) {
            console.error('Error submitting the form:', error);
          }
        }

    return (
        <form onSubmit={handleSubmit}>
            <Grid container direction="column" spacing={2}>
                <Grid item>
                    <TextField fullWidth label="Pavadinimas" variant="outlined"  value={name}
  onChange={(e) => setName(e.target.value)} />
                </Grid>
                <Grid item>
                    <TextField fullWidth label="Kiekis" variant="outlined" type="number"  value={count}
  onChange={(e) => setCount(parseInt(e.target.value, 10))} />
                </Grid>
                <Grid item>
                    <TextField fullWidth label="Apibūdinimas" variant="outlined" multiline rows={4}  value={description}
  onChange={(e) => setDescription(e.target.value)} />
                </Grid>
                {conditions.map((condition, index) => (
                    <Grid container item key={index} spacing={2} alignItems="center">
                        <Grid item>

                            <Select
                                displayEmpty
                                value={condition.Type}
                                onChange={(event) => handleConditionChange(event, index)}
                            >
                                <MenuItem value="" disabled>
                                    Sąlyga
                                </MenuItem>
                                <MenuItem value={"Any"}>Tik kiekis</MenuItem>
                                <MenuItem value={"Genre"}>Žanras</MenuItem>
                                <MenuItem value={"RuntimeMore"}>Trunka ilgiau nei min.</MenuItem>
                                <MenuItem value={"RuntimeLess"}>Trunka trumpiau nei min.</MenuItem>
                            </Select>
                        </Grid>
                        <Grid item>

                        {condition.Type === "Genre" ? (
                            <Select
                            displayEmpty
                            value={condition.Value}
                            onChange={(event) => handleInputChange(event, index, "Value")}
                            label="Reikšmė"
                            inputProps={{
                              name: "value",
                            }}
                            MenuProps={{
                                PaperProps: {
                                  style: {
                                    maxHeight: 200, // Set the maxHeight to any value you want (in pixels)
                                  },
                                },
                              }}
                          >
                            {genres.map((genre:any) => (
                              <MenuItem key={genre.id} value={genre.name}>
                                {genre.name}
                              </MenuItem>
                            ))}
                          </Select>):(

                            <TextField
                                label="Reikšmė"
                                variant="outlined"
                                value={`${condition.Value}`}
                                onChange={(e) => handleValueChange(e, index)}
                            />)}
                        </Grid>
                        <Grid item>
                            <IconButton onClick={() => removeCondition(index)} disabled={conditions.length === 1}>
                                <RemoveCircleIcon />
                            </IconButton>
                        </Grid>
                    </Grid>
                ))}
                <Grid item>
                    <Button onClick={addCondition} variant="contained" color="primary" startIcon={<AddCircleIcon />}>
                        Pridėti sąlygą
                    </Button>
                </Grid>
                <Grid item>
                    <Button type="submit" variant="contained" color="primary">
                        Sukurti
                    </Button>
                </Grid>
            </Grid>
        </form>
    );
};

export default MyForm;