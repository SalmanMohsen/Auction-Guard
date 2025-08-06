import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { Property } from '../../types';

interface PropertyState {
  properties: Property[];
  myProperties: Property[];
  loading: boolean;
  error: string | null;
}

const initialState: PropertyState = {
  properties: [],
  myProperties: [],
  loading: false,
  error: null,
};

const propertySlice = createSlice({
  name: 'property',
  initialState,
  reducers: {
    setProperties(state, action: PayloadAction<Property[]>) {
      state.properties = action.payload;
    },
    setMyProperties(state, action: PayloadAction<Property[]>) {
      state.myProperties = action.payload;
    },
    addProperty(state, action: PayloadAction<Property>) {
      state.properties.push(action.payload);
      state.myProperties.push(action.payload);
    },
    setLoading(state, action: PayloadAction<boolean>) {
      state.loading = action.payload;
    },
    setError(state, action: PayloadAction<string | null>) {
      state.error = action.payload;
    },
  },
});

export const { setProperties, setMyProperties, addProperty, setLoading, setError } = propertySlice.actions;
export default propertySlice.reducer;
