import { useState, useEffect } from 'react';

/**
 * A custom hook to manage a countdown timer.
 *
 * @param targetDate - The future date and time to count down to.
 * @returns The remaining time in milliseconds.
 */
const useCountdown = (targetDate: string | Date) => {
  const countDownDate = new Date(targetDate).getTime();

  const [countDown, setCountDown] = useState(
    countDownDate - new Date().getTime()
  );

  useEffect(() => {
    const interval = setInterval(() => {
      const remainingTime = countDownDate - new Date().getTime();
      setCountDown(remainingTime > 0 ? remainingTime : 0);
    }, 1000);

    // Cleanup the interval on component unmount
    return () => clearInterval(interval);
  }, [countDownDate]);

  return countDown;
};

export default useCountdown;
