import { ImageIcon } from "lucide-react";
import { formatCategoryLabel } from "@/lib/listings/category-feature-config";

interface ListingImagePlaceholderProps {
  categoryKey: string;
  title?: string;
  heightClassName?: string;
}

export function ListingImagePlaceholder({
  categoryKey,
  title,
  heightClassName = "h-44",
}: ListingImagePlaceholderProps): React.JSX.Element {
  return (
    <div
      className={`${heightClassName} flex items-center justify-center bg-[radial-gradient(circle_at_top_left,_rgba(110,231,183,0.8),_transparent_35%),linear-gradient(135deg,_#f0fdf4_0%,_#dcfce7_40%,_#ecfeff_100%)]`}
    >
      <div className="rounded-[1.5rem] border border-white/60 bg-white/70 px-6 py-5 text-center shadow-sm backdrop-blur-sm">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-[1rem] bg-emerald-100">
          <ImageIcon className="h-5 w-5 text-emerald-900" />
        </div>

        <p className="mt-3 text-sm font-bold text-emerald-950">
          {title ?? formatCategoryLabel(categoryKey)}
        </p>
        <p className="mt-1 text-xs text-emerald-950/55">
          Afbeelding volgt later
        </p>
      </div>
    </div>
  );
}