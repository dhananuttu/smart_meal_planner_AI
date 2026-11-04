/** Format a Date as YYYY-MM-DD in local time */
export function formatDateIso(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

/** Monday of the week containing the given date (local time) */
export function getWeekStartMonday(date: Date): string {
  const copy = new Date(date);
  const day = copy.getDay();
  const daysFromMonday = (day + 6) % 7;
  copy.setDate(copy.getDate() - daysFromMonday);
  return formatDateIso(copy);
}

export function formatDisplayDate(isoDate: string): string {
  const [year, month, day] = isoDate.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  return date.toLocaleDateString(undefined, {
    weekday: "long",
    month: "long",
    day: "numeric",
  });
}

export function addDaysToIso(isoDate: string, days: number): string {
  const [year, month, day] = isoDate.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  date.setDate(date.getDate() + days);
  return formatDateIso(date);
}

export function formatShortDay(isoDate: string): string {
  const [year, month, day] = isoDate.split("-").map(Number);
  const date = new Date(year, month - 1, day);
  return date.toLocaleDateString(undefined, {
    weekday: "short",
    month: "numeric",
    day: "numeric",
  });
}

export function isToday(isoDate: string): boolean {
  return isoDate === formatDateIso(new Date());
}
