using UnityEngine;

namespace Code.Core.Chunks
{
    /// <summary>
    /// Статический вспомогательный класс для работы с координатами и индексами чанков.
    /// Содержит функции для конвертации между мировыми и локальными координатами.
    /// </summary>
    public static class WorldMath
    {
        /// <summary>
        /// Округляет число вниз до целого.
        /// </summary>
        /// <param name="v">Вещественное значение (float).</param>
        /// <returns>Целое число (int), округлённое вниз.</returns>
        public static int FloorToInt(float v)
        {
            int i = (int)Mathf.Floor(v);
            return i;
        }

        /// <summary>
        /// Зацикливает индекс по модулю (используется для кольцевой топологии по X).
        /// Например, Wrap(-1, 10) → 9.
        /// </summary>
        /// <param name="a">Значение индекса (может быть отрицательным).</param>
        /// <param name="m">Диапазон модуля (например, количество чанков).</param>
        /// <returns>Зацикленное значение индекса [0..m-1].</returns>
        public static int Wrap(int a, int m)
        {
            int r = a % m;
            if (r < 0)
                r += m;
            return r;
        }

        /// <summary>
        /// Конвертирует мировую позицию в индекс чанка.
        /// Учитывает кольцевание по X (wrap) и ограничение по Y (clamp).
        /// </summary>
        /// <param name="wp">Мировая позиция точки (x, y).</param>
        /// <param name="cfg">Конфигурация мира (размеры чанков и сетка).</param>
        /// <returns>Структура ChunkIndex с индексами чанка (cx, cy).</returns>
        public static ChunkIndex WorldToChunkIndex(WorldPos wp, WorldConfig cfg)
        {
            int cx = FloorToInt(wp.X / cfg.chunkWidth);
            int cy = FloorToInt(wp.Y / cfg.chunkHeight);
            cx = Wrap(cx, cfg.chunksX); // кольцевание по X
            cy = Mathf.Clamp(cy, 0, cfg.chunksY - 1); // clamp по Y
            return new ChunkIndex(cx, cy);
        }

        /// <summary>
        /// Конвертирует мировую позицию в локальные координаты внутри чанка.
        /// Например, если точка находится в чанке (2,1), вернёт координаты внутри него [0..W), [0..H).
        /// </summary>
        /// <param name="wp">Мировая позиция точки (x, y).</param>
        /// <param name="cfg">Конфигурация мира.</param>
        /// <returns>Локальная позиция в чанке (lx, ly).</returns>
        public static LocalPos WorldToLocal(WorldPos wp, WorldConfig cfg)
        {
            // Вычисляем "сырые" индексы чанков без кольцевания.
            int cxRaw = FloorToInt(wp.X / cfg.chunkWidth);
            int cyRaw = FloorToInt(wp.Y / cfg.chunkHeight);

            // Вычисляем смещение внутри чанка по X.
            float lx = wp.X - cxRaw * cfg.chunkWidth;
            lx = lx % cfg.chunkWidth;
            if (lx < 0)
                lx += cfg.chunkWidth; // корректируем отрицательные значения

            // Смещение по Y.
            float ly = wp.Y - cyRaw * cfg.chunkHeight;
            ly = Mathf.Clamp(ly, 0f, cfg.chunkHeight);

            return new LocalPos(lx, ly);
        }

        /// <summary>
        /// Возвращает мировые координаты "нижнего левого угла" (origin) чанка.
        /// </summary>
        /// <param name="ci">Индекс чанка (cx, cy).</param>
        /// <param name="cfg">Конфигурация мира.</param>
        /// <returns>Позиция начала чанка в мировых координатах.</returns>
        public static Vector2 ChunkOriginWorld(ChunkIndex ci, WorldConfig cfg)
        {
            // origin (bottom-left) world coordinate of chunk ci (with wrapped cx)
            return new Vector2(ci.X * cfg.chunkWidth, ci.Y * cfg.chunkHeight);
        }

        /// <summary>
        /// Возвращает прямоугольник (AABB) чанка в мировых координатах.
        /// </summary>
        /// <param name="ci">Индекс чанка.</param>
        /// <param name="cfg">Конфигурация мира.</param>
        /// <returns>Rect — область чанка в мировых координатах.</returns>
        public static Rect ChunkAABB(ChunkIndex ci, WorldConfig cfg)
        {
            Vector2 origin = ChunkOriginWorld(ci, cfg);
            return new Rect(origin.x, origin.y, cfg.chunkWidth, cfg.chunkHeight);
        }

        /// <summary>
        /// Преобразует индекс чанка (cx, cy) в линейный индекс для массива.
        /// Полезно, если ты хранишь все чанки в одном списке/массиве.
        /// </summary>
        /// <param name="ci">Индекс чанка (cx, cy).</param>
        /// <param name="cfg">Конфигурация мира.</param>
        /// <returns>Линейный индекс чанка (0..chunksX*chunksY-1).</returns>
        public static int ToLinear(ChunkIndex ci, WorldConfig cfg)
        {
            return (int)(ci.Y * cfg.chunksX + ci.X);
        }
    }
}
