<script>
    import Header from '$lib/header/Header.svelte';
    import '../app.css';
</script>

<svelte:head>
    <title>
        Inertia game
    </title>
</svelte:head>

<Header />

<main>
    <slot />
</main>

<style>
    main {
        flex: 1;
        display: flex;
        flex-direction: column;
        padding: 1rem;
        width: 100%;
        max-width: 1024px;
        margin: 0 auto;
        box-sizing: border-box;
    }
</style>
