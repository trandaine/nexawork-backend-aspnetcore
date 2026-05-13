export default function PostCard() {
  return (
    <div className="max-w-xl mx-auto bg-white border rounded-xl shadow-sm p-4">

      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <img
            src="https://i.pravatar.cc/40"
            alt="avatar"
            className="w-10 h-10 rounded-full"
          />

          <div>
            <h2 className="font-semibold text-sm">Ooi Ming Ian</h2>
            <p className="text-xs text-gray-500">
              Graduate Student | University of Malaya
            </p>
          </div>
        </div>

        <button className="text-blue-600 font-medium text-sm hover:underline">
          + Follow
        </button>
      </div>

      {/* Time */}
      <p className="text-xs text-gray-400 mt-2">8h • 🌐</p>

      {/* Content */}
      <p className="mt-3 text-sm text-gray-700">
        We like to believe we’re rational, but much of our decision-making is
        shaped by invisible psychological forces.
      </p>

      {/* Image */}
      <div className="mt-3 border rounded-lg overflow-hidden">
        <img
          src="https://m.media-amazon.com/images/I/71Z4+F2lQ5L.jpg"
          alt="book"
          className="w-full object-cover"
        />
      </div>

      {/* Actions */}
      <div className="flex justify-between text-gray-500 text-sm mt-3 pt-3 border-t">
        <button className="hover:text-blue-600">👍 Like</button>
        <button className="hover:text-blue-600">💬 Comment</button>
        <button className="hover:text-blue-600">🔁 Repost</button>
        <button className="hover:text-blue-600">📤 Send</button>
      </div>
    </div>
  );
}