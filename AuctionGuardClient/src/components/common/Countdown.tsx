import useCountdown from '../../hooks/useCountdown';
import { formatCountdown } from '../../utils/formatters';

interface CountdownProps {
  targetDate: string | Date;
}

const Countdown = ({ targetDate }: CountdownProps) => {
  const timeRemaining = useCountdown(targetDate);

  return (
    <div className="text-2xl font-mono">
      {formatCountdown(timeRemaining)}
    </div>
  );
};

export default Countdown;
