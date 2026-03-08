import { ImagePlus, Grip, Star, Trash2 } from "lucide-react";

export function ListingImageUploaderShell(): React.JSX.Element {
  return (
    <section className="soft-panel rounded-[2rem] p-6">
      <h2 className="text-2xl font-black text-emerald-950">Afbeeldingen</h2>
      <p className="mt-2 text-sm leading-7 text-emerald-950/62">
        Voorbereid op meerdere listing-afbeeldingen met primary image, sortering en
        verwijderacties.
      </p>

      <div className="mt-6 rounded-[1.6rem] border border-dashed border-emerald-300 bg-white p-8 text-center">
        <div className="mx-auto flex h-14 w-14 items-center justify-center rounded-[1.25rem] bg-emerald-100">
          <ImagePlus className="h-6 w-6 text-emerald-900" />
        </div>

        <h3 className="mt-4 text-lg font-black text-emerald-950">
          Upload zone placeholder
        </h3>
        <p className="mt-2 text-sm leading-7 text-emerald-950/60">
          Hier komt straks drag-and-drop, browse upload en validatie voor bestandstype
          en grootte.
        </p>
      </div>

      <div className="mt-6 grid gap-4 md:grid-cols-3">
        {Array.from({ length: 3 }).map((_, index) => (
          <div
            key={index}
            className="rounded-[1.5rem] border border-emerald-200 bg-white p-4 shadow-sm"
          >
            <div className="flex h-36 items-center justify-center rounded-[1.2rem] bg-emerald-50">
              <ImagePlus className="h-6 w-6 text-emerald-900/50" />
            </div>

            <div className="mt-4 flex flex-wrap gap-2">
              <button
                type="button"
                className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-3 py-1.5 text-xs font-semibold text-emerald-950"
              >
                <Star className="h-3.5 w-3.5" />
                Primary
              </button>

              <button
                type="button"
                className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white px-3 py-1.5 text-xs font-semibold text-emerald-950"
              >
                <Grip className="h-3.5 w-3.5" />
                Sorteren
              </button>

              <button
                type="button"
                className="inline-flex items-center gap-2 rounded-full border border-red-200 bg-white px-3 py-1.5 text-xs font-semibold text-red-700"
              >
                <Trash2 className="h-3.5 w-3.5" />
                Verwijderen
              </button>
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}