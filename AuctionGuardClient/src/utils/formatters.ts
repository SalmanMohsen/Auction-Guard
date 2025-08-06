/**
 * Formats a number as a currency string.
 * @param amount - The number to format.
 * @param currency - The currency code (e.g., 'USD'). Defaults to 'USD'.
 * @returns A formatted currency string (e.g., "$1,234.56").
 */
export const formatCurrency = (amount: number, currency: string = 'USD'): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: currency,
  }).format(amount);
};

/**
 * Formats a date string or Date object into a more readable format.
 * @param date - The date to format (can be a string or Date object).
 * @param options - Optional formatting options.
 * @returns A formatted date string (e.g., "August 3, 2025, 5:52 AM").
 */
export const formatDate = (
  date: string | Date,
  options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: 'numeric',
    minute: 'numeric',
  }
): string => {
  return new Date(date).toLocaleDateString('en-US', options);
};

/**
 * Formats a date for use in a countdown timer.
 * @param date - The date to format.
 * @returns A string in a format suitable for countdowns (e.g., "days:hours:minutes:seconds").
 */
export const formatCountdown = (timeRemaining: number): string => {
    const totalSeconds = Math.floor(timeRemaining / 1000);
    const days = Math.floor(totalSeconds / (3600 * 24));
    const hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    const seconds = Math.floor(totalSeconds % 60);

    return `${days}d ${hours}h ${minutes}m ${seconds}s`;
}
