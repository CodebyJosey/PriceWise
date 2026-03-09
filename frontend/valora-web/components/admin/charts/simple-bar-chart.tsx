interface SimpleBarChartItem {
  label: string;
  value: number;
}

interface SimpleBarChartProps {
  title: string;
  items: SimpleBarChartItem[];
  valueFormatter?: (value: number) => string;
  emptyText?: string;
}

export function SimpleBarChart({
  title,
  items,
  valueFormatter = (value) => value.toString(),
  emptyText = "Geen data beschikbaar.",
}: SimpleBarChartProps): React.JSX.Element {
  const maxValue = Math.max(...items.map((item) => item.value), 0);

  return (
    <section className="soft-panel rounded-[2rem] p-6">
      <h2 className="text-2xl font-black text-emerald-950">{title}</h2>

      {items.length === 0 ? (
        <p className="mt-6 text-sm text-emerald-950/60">{emptyText}</p>
      ) : (
        <div className="mt-6 space-y-4">
          {items.map((item) => {
            const widthPercentage =
              maxValue === 0 ? 0 : Math.max((item.value / maxValue) * 100, 6);

            return (
              <div key={item.label}>
                <div className="mb-2 flex items-center justify-between gap-4">
                  <span className="truncate text-sm font-semibold text-emerald-950">
                    {item.label}
                  </span>
                  <span className="shrink-0 text-sm font-black text-emerald-900">
                    {valueFormatter(item.value)}
                  </span>
                </div>

                <div className="h-3 rounded-full bg-emerald-100">
                  <div
                    className="h-3 rounded-full bg-gradient-to-r from-emerald-400 via-lime-400 to-teal-400 transition-all"
                    style={{ width: `${widthPercentage}%` }}
                  />
                </div>
              </div>
            );
          })}
        </div>
      )}
    </section>
  );
}