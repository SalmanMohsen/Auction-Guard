interface FilterOption {
  label: string;
  value: string;
}

interface FilterProps {
  options: FilterOption[];
  activeFilter: string;
  onFilterChange: (value: string) => void;
}

const Filter = ({ options, activeFilter, onFilterChange }: FilterProps) => {
  return (
    <div className="flex space-x-2">
      {options.map(option => (
        <button
          key={option.value}
          onClick={() => onFilterChange(option.value)}
          className={`py-2 px-4 rounded text-sm font-medium ${
            activeFilter === option.value
              ? 'bg-blue-500 text-white'
              : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
          }`}
        >
          {option.label}
        </button>
      ))}
    </div>
  );
};

export default Filter;