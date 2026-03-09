interface SimpleDonutChartSegment {
  label: string;
  value: number;
  toneClassName: string;
}

interface SimpleDonutChartProps {
  title: string;
  totalLabel: string;
  segments: SimpleDonutChartSegment[];
}

export function SimpleDonutChart({
  title,
  totalLabel,
  segments,
}: SimpleDonutChartProps): React.JSX.Element {
  const total = segments.reduce((sum, segment) => sum + segment.value, 0);

  return (
    <section className="soft-panel rounded-[2rem] p-6">
      <h2 className="text-2xl font-black text-emerald-950">{title}</h2>

      <div className="mt-6 flex flex-col gap-6 lg:flex-row lg:items-center">
        <div className="relative flex h-40 w-40 items-center justify-center rounded-full bg-emerald-50">
          <div className="absolute inset-0 overflow-hidden rounded-full">
            <div className="flex h-full w-full">
              {segments.map((segment) => {
                const widthPercentage =
                  total === 0 ? 0 : (segment.value / total) * 100;

                return (
                  <div
                    key={segment.label}
                    className={segment.toneClassName}
                    style={{ width: `${widthPercentage}%` }}
                  />
                );
              })}
            </div>
          </div>

          <div className="relative z-10 flex h-24 w-24 flex-col items-center justify-center rounded-full bg-white shadow-sm">
            <span className="text-2xl font-black text-emerald-950">{total}</span>
            <span className="text-xs font-semibold uppercase tracking-[0.18em] text-emerald-900/45">
              {totalLabel}
            </span>
          </div>
        </div>

        <div className="flex-1 space-y-3">
          {segments.map((segment) => {
            const percentage = total === 0 ? 0 : (segment.value / total) * 100;

            return (
              <div
                key={segment.label}
                className="flex items-center justify-between rounded-[1.2rem] bg-white p-4 shadow-sm"
              >
                <div className="flex items-center gap-3">
                  <span
                    className={`h-3 w-3 rounded-full ${segment.toneClassName}`}
                  />
                  <span className="text-sm font-semibold text-emerald-950">
                    {segment.label}
                  </span>
                </div>

                <div className="text-right">
                  <p className="text-sm font-black text-emerald-950">
                    {segment.value}
                  </p>
                  <p className="text-xs text-emerald-900/50">
                    {percentage.toFixed(1)}%
                  </p>
                </div>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}